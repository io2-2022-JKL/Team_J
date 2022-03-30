using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("patient")]
    public class PatientController : ControllerBase
    {

        [HttpPost("timeSlots/Filter")]
        public ActionResult<IEnumerable<TimeSlotFilterResponseDTO>> FilterTimeSlots(string city, string dateFrom, string dateTo, string virus)
        {
            return NotFound();
        }

        [HttpPost("timeSlots/Book/{patientId}/{timeSlotId}")]
        public IActionResult BookVisit(string patientId, string windowId)
        {
            return NotFound();
        }

        [HttpGet("appointments/incomingAppointments/{patientId}")]
        public ActionResult<IEnumerable<FutureAppointmentDTO>> GetIncomingVisits(string patientId)
        {
            return NotFound();
        }

        [HttpDelete("appointments/IncomingAppointment/cancelAppointment/{patientId}/{appointmentId}")]
        public IActionResult CancelVisit(string appointmentId, string patientId)
        {
            return NotFound();
        }

        [HttpGet("appointments/formerAppointments/{patientId}")]
        public ActionResult<IEnumerable<FormerAppointmentDTO>> GetFormerVisits(string patientId)
        {
            return NotFound();
        }

        [HttpGet("certificates/{patientId}")]
        public ActionResult<IEnumerable<BasicCertificateInfoDTO>> GetCertificates(string patientId)
        {
            return NotFound();
        }
    }
}
