using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Config;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("patient")]
    public class PatientController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;

        private readonly ILogger<PatientController> _logger;

        public PatientController(ILogger<PatientController> logger, VaccinationSystemDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("timeSlots/Filter")]
        public ActionResult<IEnumerable<TimeSlotFilterResponseDTO>> FilterTimeSlots(string city, string dateFrom, string dateTo, string virus)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = from timeSlot in this._context.TimeSlots
                         join doctor in this._context.Doctors
                            on timeSlot.DoctorId equals doctor.Id
                         join vaccinationCenter in this._context.VaccinationCenters
                            on doctor.VaccinationCenterId equals vaccinationCenter.Id
                         where vaccinationCenter.City.Contains(city) && Convert.ToDateTime(dateFrom) <= timeSlot.From &&
                                timeSlot.To <= Convert.ToDateTime(dateTo) && vaccinationCenter.AvailableVaccines.Any(vaccine => vaccine.Virus.ToString() == virus &&
                                timeSlot.IsFree)
                         select new TimeSlotFilterResponseDTO
                         {
                             TimeSlotId = timeSlot.Id.ToString(),
                             From = timeSlot.From.ToString(),
                             To = timeSlot.To.ToString(),
                             VaccinationCenterName = vaccinationCenter.Name,
                             VaccinationCenterCity = vaccinationCenter.City,
                             VaccinationCenterStreet = vaccinationCenter.Address,
                             AvailableVaccines = 
                                vaccinationCenter.AvailableVaccines.Select(i => new SimplifiedVaccineDTO 
                                {
                                    vaccineId = i.Id.ToString(),
                                    company = i.Company,
                                    name = i.Name,
                                    numberOfDoses = i.NumberOfDoses,
                                    minDaysBetweenDoses = i.MinDaysBetweenDoses,
                                    maxDaysBetweenDoses = i.MaxDaysBetweenDoses,
                                    virus = i.Virus.ToString(),
                                    minPatientAge = i.MinPatientAge,
                                    maxPatientAge = i.MaxPatientAge,
                                }).ToList(),
                             OpeningHours = 
                                vaccinationCenter.OpeningHours.Select(i => new OpeningHoursDayDTO
                                {
                                    From = i.From.ToString(),
                                    To = i.To.ToString()
                                }).ToList(),
                             DoctorFirstName = doctor.PatientAccount.FirstName,
                             DoctorLastName = doctor.PatientAccount.LastName,
                         };
            if (result.Count() == 0) return NotFound();
            return Ok(result.AsEnumerable());
        }

        [HttpPost("timeSlots/Book/{patientId}/{timeSlotId}")]
        public IActionResult BookVisit(string patientId, string windowId)
        {
            return NotFound();
        }

        [HttpGet("appointments/incomingAppointments/{patientId}")]
        public ActionResult<IEnumerable<FutureAppointmentDTO>> GetIncomingVisits(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = from appointment in this._context.Appointments
                         where appointment.Id.ToString() == patientId && appointment.State == Models.AppointmentState.Planned
                         join timeSlot in this._context.TimeSlots
                            on appointment.TimeSlotId equals timeSlot.Id
                         join doctor in this._context.Doctors
                             on timeSlot.DoctorId equals doctor.Id
                         join vaccinationCenter in this._context.VaccinationCenters
                            on doctor.VaccinationCenterId equals vaccinationCenter.Id
                         join vaccine in this._context.Vaccines
                            on appointment.VaccineId equals vaccine.Id
                         select new FutureAppointmentDTO
                         {
                             VaccineName = vaccine.Name,
                             VaccineCompany = vaccine.Company,
                             VaccineVirus = vaccine.Virus.ToString(), // TODO: Check if this is the correct way of doing this
                             WhichVaccineDose = appointment.WhichDose,
                             AppointmentId = appointment.Id.ToString(),
                             WindowBegin = timeSlot.From.ToString(),
                             WindowEnd = timeSlot.To.ToString(),
                             VaccinationCenterName = vaccinationCenter.Name,
                             VaccinationCenterCity = vaccinationCenter.City,
                             VaccinationCenterStreet = vaccinationCenter.Address,
                             DoctorFirstName = doctor.PatientAccount.FirstName,
                             DoctorLastName = doctor.PatientAccount.LastName
                         };
            if (result.Count() == 0) return NotFound();
            return Ok(result.AsEnumerable());
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
