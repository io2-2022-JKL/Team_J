using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO.DoctorDTOs;
using VaccinationSystem.DTO;
using VaccinationSystem.Config;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;

        private readonly ILogger<DoctorController> _logger;

        public DoctorController(ILogger<DoctorController> logger, VaccinationSystemDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpPost("timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<ExistingTimeSlotDTO>> GetExistingAppointments(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<ExistingTimeSlotDTO> result = fetchExistingAppointments(doctorId);
            if (result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<ExistingTimeSlotDTO> fetchExistingAppointments(string doctorId)
        {
            var result = from appointment in this._context.Appointments
                         join timeSlot in this._context.TimeSlots
                            on appointment.TimeSlotId equals timeSlot.Id
                         where timeSlot.DoctorId == Guid.Parse(doctorId) && timeSlot.Active
                         select new ExistingTimeSlotDTO
                         {
                             Id = timeSlot.Id.ToString(),
                             From = timeSlot.From.ToString(),
                             To = timeSlot.To.ToString(),
                             IsFree = timeSlot.IsFree,
                         };
            return result.AsEnumerable();
        }
        [HttpPost("timeSlots/create/{doctorId}")]
        public IActionResult CreateAppointments(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (!createNewAppointments(doctorId, createNewVisitsRequestDTO)) return BadRequest();
            return Ok();
        }
        private bool createNewAppointments(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            DateTime currentFrom = DateTime.Parse(createNewVisitsRequestDTO.from);
            TimeSpan increment = TimeSpan.FromMinutes(createNewVisitsRequestDTO.timeSlotDurationInMinutes);
            DateTime currentTo = currentFrom + increment;
            DateTime endTo = DateTime.Parse(createNewVisitsRequestDTO.to);
            var result = from timeSlot in this._context.TimeSlots
                         where timeSlot.DoctorId == Guid.Parse(doctorId) && timeSlot.Active == true
                         select timeSlot; // my beautiful baby <3
            while (currentTo <= endTo)
            {
                var tempResult = from timeSlot in result
                                 where (timeSlot.From < currentFrom && currentFrom < timeSlot.To) ||
                                 (timeSlot.From < currentTo && currentTo < timeSlot.To) ||
                                 (currentFrom < timeSlot.From && timeSlot.To < currentTo) ||
                                 (timeSlot.From < currentFrom && currentTo < timeSlot.To)
                                 select timeSlot.Id;  // my beautiful baby <3
                if (tempResult.Count() == 0) // no colliding time slots
                {
                    this._context.TimeSlots.Add(new Models.TimeSlot
                    {
                        From = currentFrom,
                        To = currentTo,
                        DoctorId = Guid.Parse(doctorId),
                        IsFree = true,
                        Active = true,
                    });
                }
                currentTo += increment;
                currentFrom += increment;
            }
            return true;
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
