using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Config;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.Models;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;

        public DefaultController(VaccinationSystemDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(RegisterRequestDTO registerRequestDTO)
        {
            var result = AddNewUser(registerRequestDTO);
            return result;
        }

        private IActionResult AddNewUser(RegisterRequestDTO registerRequestDTO)
        {
            Patient patient = new Patient();
            patient.PESEL = registerRequestDTO.PESEL;
            if(patient.PESEL == null || patient.PESEL.Length != 11 || !long.TryParse(patient.PESEL, out _))
            {
                return BadRequest();
            }
            if(_context.Patients.Where(u=> u.PESEL == patient.PESEL && u.Active == true ).Any())
            {
                return BadRequest();
            }
            patient.FirstName = registerRequestDTO.names;
            if(patient.FirstName == null || patient.FirstName.Length == 0 || ContainsSymbol(patient.FirstName))
            {
                return BadRequest();
            }
            patient.LastName = registerRequestDTO.surname;
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
            _context.Patients.Add(patient);
            _context.SaveChanges();
            return Ok();
        }

        private bool ContainsSymbol(string s)
        {
            return s.IndexOfAny("!@#$%^&*()_+=[{]};:\"\\|<>,./".ToCharArray()) != -1;
        }
        private bool IsPhoneNumber(string s)
        {
            
            if (s.Length > 15)
                return false;
            if (s[0] == '+')
            {
                if (!long.TryParse(s.Substring(1), out _))
                    return false;
            }
            else
            {
                if (!long.TryParse(s, out _))
                    return false;
            }
            return true;
        }

        [HttpPost("signin")]
        public ActionResult<SigninResponseDTO> SignInUser(SigninRequestDTO signinRequestDTO)
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
            account = _context.Patients.SingleOrDefault(patient => patient.Active == true && patient.Mail == signinRequestDTO.mail && patient.Password == signinRequestDTO.password);
            if (account != null)
            {
                var docAccount = _context.Doctors.Include("Patients").SingleOrDefault(doc => doc.Active == true && doc.PatientId == account.Id);
                if (docAccount != null)
                {
                    result.userID = docAccount.Id.ToString();
                    result.userType = "doctor";
                    return result;
                }
                result.userID = account.Id.ToString();
                result.userType = "patient";
                return result;
            }
            account = _context.Admins.SingleOrDefault(admin => admin.Mail == signinRequestDTO.mail && admin.Password == signinRequestDTO.password);
            if (account != null)
            {
                result.userID = account.Id.ToString();
                result.userType = "admin";
                return result;
            }
            return null;
        }

        [HttpPost("user/logout/{userId}")]
        public IActionResult LogOutUser(string userId)
        {
            return NotFound();
        }


    }
}
