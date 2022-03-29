using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.Service;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly TestService _service;

        public DefaultController(TestService service)
        {
            _service = service;
        }

        [HttpGet("test")]
        public ActionResult<IEnumerable<CertificatesResponseDTO>> GetAllCertificates()
        {
            var result = _service.Test();
            if (result != null)
                return Ok(result);
            else
                return NotFound();
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
