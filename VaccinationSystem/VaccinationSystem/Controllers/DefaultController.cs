using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {

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
