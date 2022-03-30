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
        [HttpPost("timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<ExistingTimeSlotDTO>> GetExistingAppointments(string doctorId)
        {
            return NotFound();
        }

        [HttpPost("timeSlots/create/{doctorId}")]
        public IActionResult CreateAppointments(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("timeSlots/Delete/{doctorId}")]
        public IActionResult DeleteAppointment(string doctorId, IEnumerable<string> ids)
        {
            return NotFound();
        }

        [HttpPost("timeSlots/modify/{doctorId}/{timeSlotId}")]
        public IActionResult ModifyAppointment(string doctorId, string timeSlotId, ModifyTimeSlotRequestDTO modifyVisitRequestDTO)
        {
            return NotFound();
        }

        [HttpGet("formerAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorFormerAppointmentDTO>> GetFormerAppointments(string doctorId)
        {
            return NotFound();
        }

        [HttpGet("incomingAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorIncomingAppointmentDTO>> GetIncomingAppointments(string doctorId)
        {
            return NotFound();
        }

        [HttpGet("vaccinate/{doctorId}/{appointmentId}")]
        public ActionResult<DoctorMarkedAppointmentResponseDTO> GetIncomingAppointment(string doctorId, string appointmentId)
        {
            return NotFound();
        }

        [HttpPost("vaccinate/confirmVaccination/{doctorId}/{appointmentId}/{batchId}")]
        public ActionResult<DoctorConfirmVaccinationResponseDTO> ConfirmVaccination(string doctorId, string appointmentId, string batchId)
        {
            return NotFound();
        }

        [HttpPost("vaccinate/vaccinationDidNotHappen/{doctorId}/{appointmentId}")]
        public IActionResult VaccinationDidNotHappen(string doctorId, string appointmentId)
        {
            return NotFound();
        }

        [HttpPost("vaccinate/certify/{doctorId}/{appointmentId}")]
        public IActionResult Certify(string doctorId, string appointmentId)
        {
            return NotFound();
        }

        [HttpPost("vaccinate/endAppointment/{doctorId}/{appointmentId}")]
        public IActionResult EndVisit(string doctorId, string appointmentId)
        {
            return NotFound();
        }
    }
}
