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

        [HttpPost("timeSlot/filter/{patientId}")]
        public ActionResult<IEnumerable<TimeSlotFilterResponseDTO>> FilterTimeSlots(TimeSlotFilterRequestDTO timeSlotFilterRequestDTO)
        {
            return NotFound();
        }

        [HttpGet("timeSlot/Filter/{patientId}/{dayId}")]
        public ActionResult<IEnumerable<AppointmentsFilterForSpecificDayResponseDTO>> FilterDay(string patientId, string dayId)
        {
            return NotFound();
        }

        [HttpPost("timeSlot/Book/{patientId}/{windowId}")]
        public IActionResult BookVisit(string patientId, string windowId)
        {
            return NotFound();
        }

        [HttpGet("appointments/IncomingVisits/{patientId}")]
        public ActionResult<IEnumerable<FutureAppointmentResponseDTO>> GetIncomingVisits(string patientId)
        {
            return NotFound();
        }

        [HttpDelete("appointments/IncomingVisists/CancelVisit/{visitId}/{patientId}")]
        public IActionResult CancelVisit(string visitId, string patientId)
        {
            return NotFound();
        }

        [HttpGet("appointments/FormerVisits/{patientId}")]
        public ActionResult<IEnumerable<FormerAppointmentResponseDTO>> GetFormerVisits(string patientId)
        {
            return NotFound();
        }

        [HttpGet("certificates/{patientId}")]
        public ActionResult<IEnumerable<CertificatesResponseDTO>> GetCertificates(string patientId)
        {
            return NotFound();
        }

        [HttpPost("certificates/Certificate/{patientId}")]
        public ActionResult<string> GetCertificate(string patientId, CertificateToDownloadRequestDTO certificateToDownloadRequestDTO)
        {
            return NotFound();
        }
    }
}
