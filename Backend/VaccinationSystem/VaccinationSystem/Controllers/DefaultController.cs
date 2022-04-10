using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            if(patient.PESEL.Length != 11 || !int.TryParse(patient.PESEL, out _))
            {
                return StatusCode(400);
            }
            if(_context.Patients.Where(u=>u.PESEL == patient.PESEL).Any())
            {
                return StatusCode(400);
            }
            patient.FirstName = registerRequestDTO.names;
            if(ContainsSymbol(patient.FirstName))
            {
                return StatusCode(400);
            }
            patient.LastName = registerRequestDTO.surname;
            if(ContainsSymbol(patient.LastName))
            {
                return StatusCode(400);
            }
            try
            {
                patient.DateOfBirth = DateTime.ParseExact(registerRequestDTO.dateOfBirth, "dd-MM-yyyy", null);
            }
            catch(FormatException)
            {
                return StatusCode(400);
            }
            patient.Mail = registerRequestDTO.mail;
            if(!new EmailAddressAttribute().IsValid(patient.Mail))
            {
                return StatusCode(400);
            }
            patient.Password = registerRequestDTO.password;
            patient.PhoneNumber = new string(registerRequestDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            if(!IsPhoneNumber(patient.PhoneNumber))
            {
                return StatusCode(400);
            }
            patient.Vaccinations = new List<Appointment>();
            patient.Certificates = new List<Certificate>();
            patient.Active = true;
            var entry = _context.Patients.Add(patient);
            if(entry.State != Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                return StatusCode(400);
            }
            if(_context.SaveChanges() != 1)
            {
                return StatusCode(400);
            }
            return StatusCode(200);
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
                if (!int.TryParse(s.Substring(1), out _))
                    return false;
            }
            else
            {
                if (!int.TryParse(s, out _))
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
            return StatusCode(400);
        }

        private SigninResponseDTO FindUser(SigninRequestDTO signinRequestDTO)
        {
            SigninResponseDTO result = new SigninResponseDTO();
            User account;
            if ((account = _context.Doctors.Where(doc => doc.PatientAccount.Active == true && doc.PatientAccount.Mail == signinRequestDTO.mail && doc.PatientAccount.Password == signinRequestDTO.password).SingleOrDefault().PatientAccount) != null)
            {
                result.userID = account.Id.ToString();
                result.userType = "doctor";
                return result;
            }
            if((account = _context.Patients.Where(patient => patient.Active == true && patient.Mail == signinRequestDTO.mail && patient.Password == signinRequestDTO.password).SingleOrDefault()) != null)
            {
                result.userID = account.Id.ToString();
                result.userType = "patient";
                return result;
            }
            if((account = _context.Admins.Where(admin => admin.Mail == signinRequestDTO.mail && admin.Password == signinRequestDTO.password).SingleOrDefault()) != null)
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
