using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO.DoctorDTOs;
using VaccinationSystem.DTO;
using VaccinationSystem.Config;
using VaccinationSystem.Models;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;

        public DoctorController(VaccinationSystemDbContext context)
        {
            _context = context;
        }
        [HttpPost("timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<ExistingTimeSlotDTO>> GetExistingTimeSlots(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<ExistingTimeSlotDTO> result = fetchExistingTimeSlots(doctorId);
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<ExistingTimeSlotDTO> fetchExistingTimeSlots(string doctorId)
        {
            Guid docId;
            try
            {
                docId = Guid.Parse(doctorId);
            }
            catch(FormatException)
            {
                return null;
            }
            List<ExistingTimeSlotDTO> result = new List<ExistingTimeSlotDTO>();
            var timeSlots = _context.TimeSlots.Where(ts => ts.DoctorId == docId && ts.Active == true);
            foreach(TimeSlot timeSlot in timeSlots)
            {
                ExistingTimeSlotDTO existingTimeSlotDTO = new ExistingTimeSlotDTO();
                existingTimeSlotDTO.Id = timeSlot.Id.ToString();
                existingTimeSlotDTO.From = timeSlot.From.ToString();
                existingTimeSlotDTO.To = timeSlot.To.ToString();
                existingTimeSlotDTO.IsFree = timeSlot.IsFree;
                result.Add(existingTimeSlotDTO);
            }
            return result;
        }
        [HttpPost("timeSlots/create/{doctorId}")]
        public IActionResult CreateTimeSlots(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (createNewVisitsRequestDTO.timeSlotDurationInMinutes == 0) return BadRequest();
            if (!createNewTimeSlots(doctorId, createNewVisitsRequestDTO)) return BadRequest();
            return Ok();
        }
        private bool createNewTimeSlots(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            int addedTimeSlotsCount = 0;
            Guid docId;
            DateTime currentFrom, currentTo, endTo;
            TimeSpan increment;
            try
            {
                docId = Guid.Parse(doctorId);
                currentFrom = DateTime.Parse(createNewVisitsRequestDTO.from);
                increment = TimeSpan.FromMinutes(createNewVisitsRequestDTO.timeSlotDurationInMinutes);
                endTo = DateTime.Parse(createNewVisitsRequestDTO.to);
            }
            catch(FormatException)
            {
                return false;
            }
            currentTo = currentFrom + increment;
            var existingTimeSlots = _context.TimeSlots.Where(ts => ts.Active == true && ts.DoctorId == docId);
            while (currentTo <= endTo)
            {
                var tempResult = existingTimeSlots.Where(ts => (ts.From <= currentFrom && currentFrom < ts.To) ||
                                 (ts.From < currentTo && currentTo <= ts.To) ||
                                 (currentFrom <= ts.From && ts.To <= currentTo) ||
                                 (ts.From <= currentFrom && currentTo <= ts.To));
                if (tempResult.Count() == 0) // no colliding time slots
                {
                    var newTimeSlot = new TimeSlot();
                    newTimeSlot.From = currentFrom;
                    newTimeSlot.To = currentTo;
                    newTimeSlot.DoctorId = docId;
                    newTimeSlot.IsFree = true;
                    newTimeSlot.Active = true;
                    _context.TimeSlots.Add(newTimeSlot);
                    _context.SaveChanges();
                    addedTimeSlotsCount++;
                }
                currentTo += increment;
                currentFrom += increment;
            }
            return (addedTimeSlotsCount > 0);
        }

        [HttpDelete("timeSlots/Delete/{doctorId}")]
        public IActionResult DeleteAppointment(string doctorId, IEnumerable<string> ids)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (!modifyDeleteAppointment(doctorId, ids)) return NotFound();
            return Ok();
        }

        private bool modifyDeleteAppointment(string doctorId, IEnumerable<string> ids)
        {
            // Disallow deleting timeSlots that already passed?
            foreach (string id in ids)
            {
                var tempTimeSlot = this._context.TimeSlots.SingleOrDefault(ts => ts.DoctorId == Guid.Parse(doctorId) && ts.Id == Guid.Parse(id));
                if (tempTimeSlot == null) continue; // Maybe throw an error or something?
                var possibleAppointment = this._context.Appointments.SingleOrDefault(a => a.TimeSlotId == tempTimeSlot.Id && a.State == Models.AppointmentState.Planned);
                if (possibleAppointment != null)
                {
                    possibleAppointment.State = Models.AppointmentState.Cancelled;
                    // TODO: Take care of patient assigned to the appointment
                }
                tempTimeSlot.Active = false;
                this._context.SaveChanges();
            }
            return true;
        }

        [HttpPost("timeSlots/modify/{doctorId}/{timeSlotId}")]
        public IActionResult ModifyAppointment(string doctorId, string timeSlotId, ModifyTimeSlotRequestDTO modifyVisitRequestDTO)
        {
            return NotFound();
        }

        [HttpGet("formerAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorFormerAppointmentDTO>> GetFormerAppointments(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<DoctorFormerAppointmentDTO> result = fetchFormerAppointments(doctorId);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<DoctorFormerAppointmentDTO> fetchFormerAppointments(string doctorId)
        {
            var result = from appointment in this._context.Appointments
                         join timeSlot in this._context.TimeSlots
                         on appointment.TimeSlotId equals timeSlot.Id
                         join vaccine in this._context.Vaccines
                         on appointment.VaccineId equals vaccine.Id
                         join patient in this._context.Patients
                         on appointment.PatientId equals patient.Id
                         where timeSlot.DoctorId == Guid.Parse(doctorId) && appointment.State == Models.AppointmentState.Finished
                         select new DoctorFormerAppointmentDTO
                         {
                             VaccineName = vaccine.Name,
                             VaccineCompany = vaccine.Company,
                             VaccineVirus = vaccine.Virus.ToString(),
                             WhichVaccineDose = appointment.WhichDose,
                             AppointmentId = appointment.Id.ToString(),
                             PatientFirstName = patient.FirstName,
                             PatientLastName = patient.LastName,
                             PESEL = patient.PESEL,
                             State = appointment.State.ToString(),
                             BatchNumber = appointment.VaccineBatchNumber,
                             From = timeSlot.From.ToString(),
                             To = timeSlot.To.ToString(),
                         };
            return result.AsEnumerable();

        }
        [HttpGet("incomingAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorIncomingAppointmentDTO>> GetIncomingAppointments(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<DoctorIncomingAppointmentDTO> result = fetchIncomingAppointments(doctorId);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<DoctorIncomingAppointmentDTO> fetchIncomingAppointments(string doctorId)
        {
            var result = from appointment in this._context.Appointments
                         join timeSlot in this._context.TimeSlots
                         on appointment.TimeSlotId equals timeSlot.Id
                         join vaccine in this._context.Vaccines
                         on appointment.VaccineId equals vaccine.Id
                         join patient in this._context.Patients
                         on appointment.PatientId equals patient.Id
                         where timeSlot.DoctorId == Guid.Parse(doctorId) && appointment.State == Models.AppointmentState.Planned && patient.Active && timeSlot.Active && vaccine.Active
                         select new DoctorIncomingAppointmentDTO
                         {
                             VaccineName = vaccine.Name,
                             VaccineCompany = vaccine.Company,
                             VaccineVirus = vaccine.Virus.ToString(),
                             WhichVaccineDose = appointment.WhichDose,
                             AppointmentId = appointment.Id.ToString(),
                             PatientFirstName = patient.FirstName,
                             PatientLastName = patient.LastName,
                             From = timeSlot.From.ToString(),
                             To = timeSlot.To.ToString(),
                         };
            return result.AsEnumerable();

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
