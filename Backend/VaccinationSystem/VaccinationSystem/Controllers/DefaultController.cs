using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VaccinationSystem.Config;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.Models;

namespace VaccinationSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUri = "https://systemszczepienkonta.azurewebsites.net/";

        public DefaultController(VaccinationSystemDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        /// <remarks>
        /// Registers a patient
        /// </remarks>
        /// <param name="registerRequestDTO"></param>
        /// <response code="200">OK, successfully registered</response>
        /// <response code="400">Error, user sent incomplete data</response>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult RegisterUser([FromBody, Required]RegisterRequestDTO registerRequestDTO)
        {
            var result = AddNewUser(registerRequestDTO).Result;
            return result;
        }

        private async Task<IActionResult> AddNewUser(RegisterRequestDTO registerRequestDTO)
        {
            Patient patient = new Patient();
            patient.PESEL = registerRequestDTO.PESEL;
            if(patient.PESEL == null || patient.PESEL.Length != 11 || !ulong.TryParse(patient.PESEL, out _))
            {
                return BadRequest();
            }
            if(_context.Patients.Where(u=> u.PESEL == patient.PESEL && u.Active == true ).Any())
            {
                return BadRequest();
            }
            patient.FirstName = registerRequestDTO.firstName;
            if(patient.FirstName == null || patient.FirstName.Length == 0 || ContainsSymbol(patient.FirstName))
            {
                return BadRequest();
            }
            patient.LastName = registerRequestDTO.lastName;
            if(patient.LastName == null || patient.LastName.Length == 0 || ContainsSymbol(patient.LastName))
            {
                return BadRequest();
            }
            try
            {
                patient.DateOfBirth = DateTime.ParseExact(registerRequestDTO.dateOfBirth, "dd-MM-yyyy", null);
            }
            catch(FormatException)
            {
                return BadRequest();
            }
            catch(ArgumentNullException)
            {
                return BadRequest();
            }
            patient.Mail = registerRequestDTO.mail;
            if(patient.Mail == null || !new EmailAddressAttribute().IsValid(patient.Mail))
            {
                return BadRequest();
            }
            if (_context.Patients.Where(u => u.Mail == patient.Mail && u.Active == true).Any())
            {
                return BadRequest();
            }
            patient.Password = registerRequestDTO.password;
            if(patient.Password == null || patient.Password.Length == 0)
            {
                return BadRequest();
            }
            if(registerRequestDTO.phoneNumber == null)
            {
                return BadRequest();
            }
            patient.PhoneNumber = new string(registerRequestDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            if(patient.PhoneNumber.Length == 0 || !IsPhoneNumber(patient.PhoneNumber))
            {
                return BadRequest();
            }
            //patient.Vaccinations = new List<Appointment>();
            //patient.Certificates = new List<Certificate>();
            patient.Active = true;
            patient.Id = Guid.NewGuid();

            var registerISDTO = new RegisterInIdentityServerDTO()
            {
                userId = patient.Id.ToString(),
                email = patient.Mail,
                phoneNumber = patient.PhoneNumber,
                role = Role.Patient,
                password = patient.Password,
            };
            var registerJSON = JsonSerializer.Serialize(registerISDTO);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "user/register");
            httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpRequestMessage.Content = new StringContent(registerJSON, System.Text.Encoding.UTF8);
            httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_baseUri);

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (!httpResponseMessage.IsSuccessStatusCode)
                return BadRequest();

            _context.Patients.Add(patient);
            _context.SaveChanges();
            return Ok();
        }

        public static bool ContainsSymbol(string s)
        {
            return s.IndexOfAny("!@#$%^&*()_+=[{]};:\"\\|<>,./".ToCharArray()) != -1;
        }
        public static bool IsPhoneNumber(string s)
        {
            
            if (s.Length > 15)
                return false;
            if (s[0] == '+')
            {
                if (!ulong.TryParse(s.Substring(1), out _))
                    return false;
            }
            else
            {
                if (!ulong.TryParse(s, out _))
                    return false;
            }
            return true;
        }

        /// <remarks>Logs in user</remarks>
        /// <param name="signinRequestDTO"></param>
        /// <response code="200">OK, successfully signed up</response>
        /// <response code="400">Error, user doesn't exists</response>
        [AllowAnonymous]
        [HttpPost("signin")]
        [ProducesResponseType(typeof(SigninResponseDTO), 200)]
        [ProducesResponseType(400)]
        public ActionResult<SigninResponseDTO> SignInUser([FromBody, Required]SigninRequestDTO signinRequestDTO)
        {
            var result = FindUser(signinRequestDTO);
            if (result != null)
                return Ok(result);
            return BadRequest();
        }

        private SigninResponseDTO FindUser(SigninRequestDTO signinRequestDTO)
        {
            SigninResponseDTO result = new SigninResponseDTO();
            User account;
            string token;
            account = _context.Patients.SingleOrDefault(patient => patient.Active == true && patient.Mail == signinRequestDTO.mail && patient.Password == signinRequestDTO.password);
            if (account != null)
            {
                var docAccount = _context.Doctors.Include("Patients").SingleOrDefault(doc => doc.Active == true && doc.PatientId == account.Id);
                if (docAccount != null)
                {
                    result.userId = docAccount.Id.ToString();
                    result.userType = "doctor";
                    if ((token = GetToken(signinRequestDTO).Result) != null)
                    {
                        HttpContext.Response.Headers.Add(HeaderNames.Authorization, token);
                        return result;
                    }
                }
                result.userId = account.Id.ToString();
                result.userType = "patient";
                if ((token = GetToken(signinRequestDTO).Result) != null)
                {
                    HttpContext.Response.Headers.Add(HeaderNames.Authorization, token);
                    return result;
                }
            }
            account = _context.Admins.SingleOrDefault(admin => admin.Mail == signinRequestDTO.mail && admin.Password == signinRequestDTO.password);
            if (account != null)
            {
                result.userId = account.Id.ToString();
                result.userType = "admin";
                if ((token = GetToken(signinRequestDTO).Result) != null)
                {
                    HttpContext.Response.Headers.Add(HeaderNames.Authorization, token);
                    return result;
                }
            }
            return null;
        }

        private async Task<string> GetToken(SigninRequestDTO signinRequestDTO)
        {
            var request = new TokenRequestDTO()
            {
                client_id = "team-j-client",
                client_secret = "secret",
                grant_type = "password",
                username = signinRequestDTO.mail,
                password = signinRequestDTO.password
            };
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "connect/token")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", request.client_id),
                    new KeyValuePair<string, string>("client_secret", request.client_secret),
                    new KeyValuePair<string, string>("grant_type", request.grant_type),
                    new KeyValuePair<string, string>("username", request.username),
                    new KeyValuePair<string, string>("password", request.password)
                })
            };
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_baseUri);
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if(httpResponseMessage.IsSuccessStatusCode)
            {
                var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponseDTO>(jsonString);
                if(tokenResponse != null && tokenResponse.access_token != null && tokenResponse.access_token.Length != 0)
                    return tokenResponse.access_token;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        /// <remarks>Returns list of all viruses</remarks>
        /// <response code="200">OK</response>
        /// <response code="404">Not found</response>
        [AllowAnonymous]
        [HttpGet("viruses")]
        [ProducesResponseType(typeof(IEnumerable<GetVirusDTO>), 200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<GetVirusDTO>> GetViruses()
        {
            var result = GetAllVirusesNames();
            if (result.Count() == 0)
                return NotFound();
            return Ok(result);
        }

        private IEnumerable<GetVirusDTO> GetAllVirusesNames()
        {
            List<GetVirusDTO> result = new List<GetVirusDTO>();
            foreach(var virus in Enum.GetValues(typeof(Virus)).Cast<Virus>())
            {
                GetVirusDTO virusDTO = new GetVirusDTO()
                {
                    virus = virus.ToString()
                };
                result.Add(virusDTO);
            }
            return result;
        }

        /// <remarks>Returns list of all cities</remarks>
        /// <response code="200">OK</response>
        /// <response code="404">Not found</response>
        [AllowAnonymous]
        [HttpGet("cities")]
        [ProducesResponseType(typeof(IEnumerable<GetCitiesDTO>), 200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<GetCitiesDTO>> GetCities()
        {
            var result = GetAllCities();
            if (result.Count() == 0)
                return NotFound();
            return Ok(result);
        }

        private IEnumerable<GetCitiesDTO> GetAllCities()
        {
            List<GetCitiesDTO> result = new List<GetCitiesDTO>();
            foreach(var city in _context.VaccinationCenters.Select(vc => vc.City).Distinct())
            {
                GetCitiesDTO cityDTO = new GetCitiesDTO()
                {
                    city = city
                };
                result.Add(cityDTO);
            }
            return result;
        }

    }
}
