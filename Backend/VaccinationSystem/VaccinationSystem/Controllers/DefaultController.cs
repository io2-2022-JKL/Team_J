using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            return NotFound();
        }

        [HttpPost("signin")]
        public ActionResult<SigninResponseDTO> SignInUser(SigninRequestDTO signinRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("user/logout/{userId}")]
        public IActionResult LogOutUser(string userId)
        {
            return NotFound();
        }


    }
}
