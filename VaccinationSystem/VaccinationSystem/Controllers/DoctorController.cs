using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO.DoctorDTOs;
using VaccinationSystem.DTO;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("doctor")]
    public class DoctorController : ControllerBase
    {
        [HttpPost("createAppointments/GetExistingAppointments/{doctorId}")]
        public ActionResult<IEnumerable<AppointmentsFilterForSpecificDayResponseDTO>> GetExistingAppointments(string doctorId, GetExistingDoctorAppointmentsRequestDTO getExistingDoctorAppointmentsRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("createAppointments/Create/{doctorId}")]
        public IActionResult CreateAppointments(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("createAppointments/Delete/{doctorId}/{windowId}")]
        public IActionResult DeleteAppointment(string doctorId, string windowId)
        {
            return NotFound();
        }

        [HttpPost("createAppointments/Modify/{doctorId}/{windowId}")]
        public IActionResult ModifyAppointment(string doctorId, string windowId, ModifyVisitRequestDTO modifyVisitRequestDTO)
        {
            return NotFound();
        }

        [HttpGet("appointments/IncomingVisits/{doctorId}")]
        public ActionResult<IEnumerable<DoctorIncomingVisitsResponseDTO>> GetIncomingVisits(string doctorId)
        {
            return NotFound();
        }

        [HttpGet("appointments/IncomingVisits/{doctorId}/{visitId}")]
        public ActionResult<DoctorIncomingSpecificVisitResponseDTO> GetIncomingVisit(string doctorId, string visitId)
        {
            return NotFound();
        }

        [HttpPost("Vaccinate/ConfirmVaccination/{doctorId}/{visitId}/{batchId}")]
        public ActionResult<DoctorConfirmVaccinationResponseDTO> ConfirmVaccination(string doctorId, string visitId, string batchId)
        {
            return NotFound();
        }

        [HttpPost("Vaccinate/VaccinationDidNotHappen/{doctorId}/{visitId}")]
        public IActionResult VaccinationDidNotHappen(string doctorId, string visitId)
        {
            return NotFound();
        }

        [HttpPost("Vaccinate/Certify/{doctorId}/{visitId}")]
        public IActionResult Certify(string doctorId, string visitId)
        {
            return NotFound();
        }

        [HttpPost("Vaccinate/EndVisit/{doctorId}/{visitId}")]
        public IActionResult EndVisit(string doctorId, string visitId)
        {
            return NotFound();
        }
    }
}
