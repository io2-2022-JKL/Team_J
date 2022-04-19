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
using System.Data.Entity;

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
        [HttpGet("{doctorId}/patientId")]
        public ActionResult<GetDoctorPatientIdResponse> GetDoctorPatientId(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            GetDoctorPatientIdResponse result = FetchDoctorPatientId(doctorId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        private GetDoctorPatientIdResponse FetchDoctorPatientId(string doctorId)
        {
            Guid docId;
            try
            {
                docId = Guid.Parse(doctorId);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            var doctorAccount = _context.Doctors.Where(doc => doc.Id == docId).Include(doc => doc.PatientAccount).SingleOrDefault();
            if (doctorAccount == null) return null;
            Guid patientAccountId = doctorAccount.PatientAccount.Id;
            GetDoctorPatientIdResponse result = new GetDoctorPatientIdResponse()
            {
                patientId = patientAccountId.ToString(),
            };
            return result;
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
            catch (ArgumentNullException)
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
            catch (ArgumentNullException)
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
        public IActionResult DeleteTimeSlot(string doctorId, IEnumerable<string> ids)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (!modifyDeleteTimeSlot(doctorId, ids)) return NotFound();
            return Ok();
        }

        private bool modifyDeleteTimeSlot(string doctorId, IEnumerable<string> ids)
        {
            // Disallow deleting timeSlots that already passed?
            int changedTimeSlots = 0;
            List<Guid> parsedIDs = new List<Guid>();
            Guid docId;
            try
            {
                docId = Guid.Parse(doctorId);
                foreach(string id in ids)
                {
                    Guid newGuid = Guid.Parse(id);
                    parsedIDs.Add(newGuid);
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            foreach (Guid id in parsedIDs)
            {
                var tempTimeSlot = _context.TimeSlots.Where(ts => ts.DoctorId == docId && ts.Id == id && ts.Active == true).SingleOrDefault();
                if (tempTimeSlot == null) continue;
                var possibleAppointment = this._context.Appointments.Where(a => a.TimeSlotId == tempTimeSlot.Id && a.State == Models.AppointmentState.Planned).SingleOrDefault();
                if (possibleAppointment != null)
                {
                    possibleAppointment.State = Models.AppointmentState.Cancelled;
                    // TODO: Take care of patient assigned to the appointment (email)
                }
                tempTimeSlot.Active = false;
                this._context.SaveChanges();
                changedTimeSlots++;
            }
            return changedTimeSlots > 0;
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
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<DoctorFormerAppointmentDTO> fetchFormerAppointments(string doctorId)
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
            catch(ArgumentNullException)
            {
                return null;
            }
            List<DoctorFormerAppointmentDTO> result = new List<DoctorFormerAppointmentDTO>();
            var appointments = _context.Appointments.Where(ap => ap.State != AppointmentState.Planned).Include(ap => ap.TimeSlot).Include(ap => ap.Patient).Include(ap => ap.Vaccine);
            foreach(Appointment appointment in appointments)
            {
                if (appointment.TimeSlot.Active == false || appointment.TimeSlot.DoctorId != docId) continue;
                DoctorFormerAppointmentDTO doctorFormerAppointmentDTO = new DoctorFormerAppointmentDTO();
                doctorFormerAppointmentDTO.VaccineName = appointment.Vaccine.Name;
                doctorFormerAppointmentDTO.VaccineCompany = appointment.Vaccine.Company;
                doctorFormerAppointmentDTO.VaccineVirus = appointment.Vaccine.Virus.ToString();
                doctorFormerAppointmentDTO.WhichVaccineDose = appointment.WhichDose;
                doctorFormerAppointmentDTO.AppointmentId = appointment.Id.ToString();
                doctorFormerAppointmentDTO.PatientFirstName = appointment.Patient.FirstName;
                doctorFormerAppointmentDTO.PatientLastName = appointment.Patient.LastName;
                doctorFormerAppointmentDTO.PESEL = appointment.Patient.PESEL;
                doctorFormerAppointmentDTO.State = appointment.State.ToString();
                doctorFormerAppointmentDTO.BatchNumber = appointment.VaccineBatchNumber;
                doctorFormerAppointmentDTO.From = appointment.TimeSlot.From.ToString();
                doctorFormerAppointmentDTO.To = appointment.TimeSlot.To.ToString();
                result.Add(doctorFormerAppointmentDTO);
            }
            return result;

        }
        [HttpGet("incomingAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorIncomingAppointmentDTO>> GetIncomingAppointments(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<DoctorIncomingAppointmentDTO> result = fetchIncomingAppointments(doctorId);
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<DoctorIncomingAppointmentDTO> fetchIncomingAppointments(string doctorId)
        {
            Guid docId;
            try
            {
                docId = Guid.Parse(doctorId);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            List<DoctorIncomingAppointmentDTO> result = new List<DoctorIncomingAppointmentDTO>();
            var appointments = _context.Appointments.Where(ap => ap.State == AppointmentState.Planned).Include(ap => ap.TimeSlot).Include(ap => ap.Patient).Include(ap => ap.Vaccine);
            foreach (Appointment appointment in appointments)
            {
                if (appointment.TimeSlot.Active == false || appointment.TimeSlot.DoctorId != docId ||
                    appointment.Patient.Active == false || appointment.Vaccine.Active == false) continue;
                DoctorIncomingAppointmentDTO doctorFormerAppointmentDTO = new DoctorIncomingAppointmentDTO();
                doctorFormerAppointmentDTO.VaccineName = appointment.Vaccine.Name;
                doctorFormerAppointmentDTO.VaccineCompany = appointment.Vaccine.Company;
                doctorFormerAppointmentDTO.VaccineVirus = appointment.Vaccine.Virus.ToString();
                doctorFormerAppointmentDTO.WhichVaccineDose = appointment.WhichDose;
                doctorFormerAppointmentDTO.AppointmentId = appointment.Id.ToString();
                doctorFormerAppointmentDTO.PatientFirstName = appointment.Patient.FirstName;
                doctorFormerAppointmentDTO.PatientLastName = appointment.Patient.LastName;
                doctorFormerAppointmentDTO.From = appointment.TimeSlot.From.ToString();
                doctorFormerAppointmentDTO.To = appointment.TimeSlot.To.ToString();
                result.Add(doctorFormerAppointmentDTO);
            }
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
