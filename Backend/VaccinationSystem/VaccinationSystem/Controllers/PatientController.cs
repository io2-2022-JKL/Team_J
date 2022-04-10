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
            var result = fetchFilteredTimeSlots(city, dateFrom, dateTo, virus);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<TimeSlotFilterResponseDTO> fetchFilteredTimeSlots(string city, string dateFrom, string dateTo, string virus)
        {
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
            return result.AsEnumerable();
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
            var result = fetchIncomingVisits(patientId);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }

        private IEnumerable<FutureAppointmentDTO> fetchIncomingVisits(string patientId)
        {
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
                             DoctorLastName = doctor.PatientAccount.LastName,
                         };
            return result.AsEnumerable();
        }

        [HttpDelete("appointments/IncomingAppointment/cancelAppointment/{patientId}/{appointmentId}")]
        public IActionResult CancelVisit(string appointmentId, string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (modifyCancelVisit(appointmentId, patientId)) return NotFound();
            return Ok();
        }

        private bool modifyCancelVisit(string appointmentId, string patientId)
        {
            var appointment = this._context.Appointments.SingleOrDefault(a => String.Compare(a.Id.ToString(), appointmentId) == 0 &&
                                                                         String.Compare(a.PatientId.ToString(), patientId) == 0);
            if (appointment == null) return false;
            Guid timeSlotId = appointment.TimeSlotId.GetValueOrDefault();
            if (timeSlotId == null) return false;
            var timeSlot = this._context.TimeSlots.SingleOrDefault(a => a.Id == timeSlotId);
            if (timeSlot == null) return false;
            appointment.State = Models.AppointmentState.Cancelled;
            timeSlot.IsFree = true;
            this._context.SaveChanges();
            return true;
        }



        [HttpGet("appointments/formerAppointments/{patientId}")]
        public ActionResult<IEnumerable<FormerAppointmentDTO>> GetFormerVisits(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = fetchFormerVisits(patientId);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }

        private IEnumerable<FormerAppointmentDTO> fetchFormerVisits(string patientId)
        {
            var result = from appointment in this._context.Appointments
                         where appointment.Id.ToString() == patientId && appointment.State != Models.AppointmentState.Planned
                         join timeSlot in this._context.TimeSlots
                            on appointment.TimeSlotId equals timeSlot.Id
                         join doctor in this._context.Doctors
                             on timeSlot.DoctorId equals doctor.Id
                         join vaccinationCenter in this._context.VaccinationCenters
                            on doctor.VaccinationCenterId equals vaccinationCenter.Id
                         join vaccine in this._context.Vaccines
                            on appointment.VaccineId equals vaccine.Id
                         select new FormerAppointmentDTO
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
                             DoctorLastName = doctor.PatientAccount.LastName,
                             VisitState = appointment.State.ToString(),
                         };
            return result.AsEnumerable();
        }

        [HttpGet("certificates/{patientId}")]
        public ActionResult<IEnumerable<BasicCertificateInfoDTO>> GetCertificates(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = fetchCertificates(patientId);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }

        private IEnumerable<BasicCertificateInfoDTO> fetchCertificates(string patientId)
        {
            var result = from certificate in this._context.Certificates
                         join patient in this._context.Patients
                         on certificate.PatientId equals patient.Id
                         join vaccine in this._context.Vaccines
                         on certificate.VaccineId equals vaccine.Id
                         where patient.Id.ToString() == patientId
                         select new BasicCertificateInfoDTO
                         {
                             Url = certificate.Url,
                             VaccineName = vaccine.Name,
                             VaccineCompany = vaccine.Company,
                             Virus = vaccine.Virus.ToString(),
                         };
            return result.AsEnumerable();
        }
    }
}
