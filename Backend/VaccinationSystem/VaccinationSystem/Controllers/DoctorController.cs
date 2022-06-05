using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VaccinationSystem.DTO.DoctorDTOs;
using VaccinationSystem.DTO;
using VaccinationSystem.Config;
using VaccinationSystem.Models;
using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using VaccinationSystem.DTO.Errors;
using PdfSharp.Pdf;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;
using PdfSharp.Drawing.Layout;
using QRCoder;
using System.Drawing;
using System.IO;
using Microsoft.Extensions.Configuration;
using VaccinationSystem.MailStuff;

namespace VaccinationSystem.Controllers
{
    [Authorize(Policy = "DoctorPolicy")]
    [ApiController]
    [Route("doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly string _dateTimeFormat = "dd-MM-yyyy HH\\:mm";
        private readonly string _dateFormat = "dd-MM-yyyy";
        private readonly string _storageUrlBase = "https://vaccinationsystem.blob.core.windows.net/certificates/";
        private readonly string _siteName = "systemszczepien.azurewebsites.net";
        public DoctorController(VaccinationSystemDbContext context, IMailService mailService, IConfiguration configuration)
        {
            _context = context;
            _mailService = mailService;
            _configuration = configuration;
        }
        [ProducesResponseType(typeof(IEnumerable<GetDoctorInfoResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("info/{doctorId}")]
        public ActionResult<GetDoctorInfoResponse> GetDoctorInfo(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            GetDoctorInfoResponse result;
            try
            {
                result = FetchDoctorPatientId(doctorId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == null) return NotFound();
            return Ok(result);
        }
        private GetDoctorInfoResponse FetchDoctorPatientId(string doctorId)
        {
            Guid docId;
            try
            {
                docId = Guid.Parse(doctorId);
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var doctorAccount = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).Include(doc => doc.VaccinationCenter).SingleOrDefault();
            if (doctorAccount == null) return null;
            Guid patientAccountId = doctorAccount.PatientId;

            var vaccianationCenter = _context.VaccinationCenters.SingleOrDefault(vc => vc.Id == doctorAccount.VaccinationCenterId);
            if(vaccianationCenter == null) return null;

            GetDoctorInfoResponse result = new GetDoctorInfoResponse()
            {
                patientAccountId = patientAccountId.ToString(),
                vaccinationCenterId = doctorAccount.VaccinationCenterId.ToString(),
                vaccinationCenterCity = vaccianationCenter.City,
                vaccinationCenterName = vaccianationCenter.Name,
                vaccinationCenterStreet = vaccianationCenter.Address,
            };
            return result;
        }
        [ProducesResponseType(typeof(IEnumerable<ExistingTimeSlotDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<ExistingTimeSlotDTO>> GetExistingTimeSlots(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<ExistingTimeSlotDTO> result;
            try
            {
                result = fetchExistingTimeSlots(doctorId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
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
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return null;
            List<ExistingTimeSlotDTO> result = new List<ExistingTimeSlotDTO>();
            var timeSlots = _context.TimeSlots.Where(ts => ts.DoctorId == docId && ts.Active == true).ToList();
            foreach(TimeSlot timeSlot in timeSlots)
            {
                //if (timeSlot.From < DateTime.Now) continue;
                ExistingTimeSlotDTO existingTimeSlotDTO = new ExistingTimeSlotDTO();
                existingTimeSlotDTO.id = timeSlot.Id.ToString();
                existingTimeSlotDTO.from = timeSlot.From.ToString(_dateTimeFormat);
                existingTimeSlotDTO.to = timeSlot.To.ToString(_dateTimeFormat);
                existingTimeSlotDTO.isFree = timeSlot.IsFree;
                result.Add(existingTimeSlotDTO);
            }
            return result;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [HttpPost("timeSlots/create/{doctorId}")]
        public IActionResult CreateTimeSlots(string doctorId, CreateNewVisitsRequestDTO createNewVisitsRequestDTO)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (createNewVisitsRequestDTO.timeSlotDurationInMinutes == 0) return BadRequest();
            bool result;
            try
            {
                result = createNewTimeSlots(doctorId, createNewVisitsRequestDTO);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return BadRequest();
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
                currentFrom = DateTime.ParseExact(createNewVisitsRequestDTO.windowBegin, _dateTimeFormat, null);
                increment = TimeSpan.FromMinutes(createNewVisitsRequestDTO.timeSlotDurationInMinutes);
                endTo = DateTime.ParseExact(createNewVisitsRequestDTO.windowEnd, _dateTimeFormat, null);
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            Doctor doctor = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).SingleOrDefault();
            if (doctor == null) return false;
            currentTo = currentFrom + increment;
            var existingTimeSlots = _context.TimeSlots.Where(ts => ts.Active == true && ts.DoctorId == docId).ToList(); 
            while (currentTo <= endTo)
            {
                var tempResult = existingTimeSlots.Where(ts => (ts.From <= currentFrom && currentFrom < ts.To) ||
                                 (ts.From < currentTo && currentTo <= ts.To) ||
                                 (currentFrom <= ts.From && ts.To <= currentTo) ||
                                 (ts.From <= currentFrom && currentTo <= ts.To)).Count();
                if (tempResult == 0) // no colliding time slots
                {
                    var newTimeSlot = new TimeSlot();
                    newTimeSlot.Id = Guid.NewGuid();
                    newTimeSlot.From = currentFrom;
                    newTimeSlot.To = currentTo;
                    newTimeSlot.Doctor = doctor;
                    newTimeSlot.DoctorId = docId;
                    newTimeSlot.IsFree = true;
                    newTimeSlot.Active = true;
                    _context.TimeSlots.Add(newTimeSlot);
                    addedTimeSlotsCount++;
                }
                currentTo += increment;
                currentFrom += increment;
            }
            if(addedTimeSlotsCount > 0)
            {
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("timeSlots/delete/{doctorId}")]
        public IActionResult DeleteTimeSlot(string doctorId, IEnumerable<TimeSlotToDeleteDTO> ids)
        {
            // TODO: Token verification for 401 and 403 error codes
            bool result;
            try
            {
                result = tryDeleteTimeSlot(doctorId, ids);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return NotFound();
            return Ok();
        }

        private bool tryDeleteTimeSlot(string doctorId, IEnumerable<TimeSlotToDeleteDTO> ids)
        {
            // Disallow deleting timeSlots that already passed?
            int changedTimeSlots = 0;
            List<Guid> parsedIDs = new List<Guid>();
            Guid docId;
            try
            {
                docId = Guid.Parse(doctorId);
                foreach(TimeSlotToDeleteDTO id in ids)
                {
                    Guid newGuid = Guid.Parse(id.id);
                    parsedIDs.Add(newGuid);
                }
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return false;
            foreach (Guid id in parsedIDs)
            {
                var tempTimeSlot = _context.TimeSlots.Where(ts => ts.DoctorId == docId && ts.Id == id && ts.Active == true).SingleOrDefault();
                if (tempTimeSlot == null) continue;

                /*var possibleAppointment = this._context.Appointments.Include(a => a.Patient).
                    Where(a => a.TimeSlotId == tempTimeSlot.Id && a.State == Models.AppointmentState.Planned).SingleOrDefault();*/
                var possibleAppointment = this._context.Appointments.Where(a => a.TimeSlotId == tempTimeSlot.Id && a.State == Models.AppointmentState.Planned).SingleOrDefault();
                if (possibleAppointment != null)
                {
                    possibleAppointment.State = Models.AppointmentState.Cancelled;
                    if (_mailService != null)
                    {
                        MailRequest request = new MailRequest();
                        request.Subject = "Visit cancelled";
                        request.Body = "Your visit from " + tempTimeSlot.From + " to " + tempTimeSlot.To + " has been cancelled.";
                        request.ToEmail = possibleAppointment.Patient.Mail;
                        try
                        {
                            _mailService.SendEmailAsync(request);
                        }
                        catch
                        {
                            throw;
                        } 
                    }
                }
                tempTimeSlot.Active = false;
                this._context.SaveChanges();
                changedTimeSlots++;
            }
            return changedTimeSlots > 0;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("timeSlots/modify/{doctorId}/{timeSlotId}")]
        public IActionResult ModifyAppointment(string doctorId, string timeSlotId, ModifyTimeSlotRequestDTO modifyVisitRequestDTO)
        {
            // TODO: Token verification for 401 and 403 error codes
            bool result;
            try
            {
                result = tryModifyAppointment(doctorId, timeSlotId, modifyVisitRequestDTO);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return NotFound();
            return Ok();
        }

        [NonAction]
        public bool tryModifyAppointment(string doctorId, string timeSlotId, ModifyTimeSlotRequestDTO modifyVisitRequestDTO)
        {
            Guid docId, tsId;
            DateTime newFrom, newTo, oldFrom, oldTo;
            try
            {
                docId = Guid.Parse(doctorId);
                tsId = Guid.Parse(timeSlotId);
                newFrom = DateTime.ParseExact(modifyVisitRequestDTO.timeFrom, _dateTimeFormat, null);
                newTo = DateTime.ParseExact(modifyVisitRequestDTO.timeTo, _dateTimeFormat, null);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            if (newTo <= newFrom) throw new BadRequestException();
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return false;

            // Find the time slot to change
            var timeSlotToChange = _context.TimeSlots.Where(ts => ts.Id == tsId && ts.DoctorId == docId && ts.Active == true).SingleOrDefault();
            if (timeSlotToChange == null) return false;

            // Check if there is a collision
            var collidingTimeSlot = _context.TimeSlots.Where(ts => ts.DoctorId == docId && ts.Active == true && ts.Id != tsId &&
                                ((ts.From <= newFrom && newFrom < ts.To) ||
                                 (ts.From < newTo && newTo <= ts.To) ||
                                 (newFrom <= ts.From && ts.To <= newTo) ||
                                 (ts.From <= newFrom && newTo <= ts.To))).ToList();
            // All time slots which are active, belong to this doctor, collide with new time slot start and end times and are NOT the time slot we're changing
            if (collidingTimeSlot != null && collidingTimeSlot.Count > 0) throw new BadRequestException(); // There are collisions

            oldFrom = timeSlotToChange.From;
            oldTo = timeSlotToChange.To;
            timeSlotToChange.From = newFrom;
            timeSlotToChange.To = newTo;
            _context.SaveChanges();
            var possibleAppointment = this._context.Appointments.Where(a => a.TimeSlotId == timeSlotToChange.Id &&
            a.State == Models.AppointmentState.Planned).Include(a => a.Patient).SingleOrDefault();
            if (possibleAppointment != null)
            {
                possibleAppointment.State = Models.AppointmentState.Cancelled;
                if (_mailService != null)
                {
                    var patient = _context.Patients.Where(p => p.Id == possibleAppointment.PatientId).SingleOrDefault();
                    MailRequest request = new MailRequest();
                    request.Subject = "Visit modified";
                    request.Body = "Your visit from " + oldFrom + " to " + oldTo + " has been changed. " +
                        "It's now from " + timeSlotToChange.From + " to " + timeSlotToChange.To + ".";
                    request.ToEmail = patient.Mail;
                    try
                    {
                        _mailService.SendEmailAsync(request);
                    }
                    catch
                    {
                        throw;
                    } 
                }
            }
            return true;
        }
        [ProducesResponseType(typeof(IEnumerable<DoctorFormerAppointmentDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("formerAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorFormerAppointmentDTO>> GetFormerAppointments(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<DoctorFormerAppointmentDTO> result;
            try
            {
                result = fetchFormerAppointments(doctorId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
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
                throw new BadRequestException();
            }
            catch(ArgumentNullException)
            {
                throw new BadRequestException();
            }

            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return null;

            List<DoctorFormerAppointmentDTO> result = new List<DoctorFormerAppointmentDTO>();
            var appointments = _context.Appointments.Where(ap => ap.State != Models.AppointmentState.Planned).Include(ap => ap.TimeSlot).Include(ap => ap.Patient).Include(ap => ap.Vaccine).ToList();
            foreach (Appointment appointment in appointments)
            {
                TimeSlot timeSlot = appointment.TimeSlot;
                Patient patient = appointment.Patient;
                Vaccine vaccine = appointment.Vaccine;
                if(timeSlot == null)
                {
                    timeSlot = _context.TimeSlots.Where(ts => ts.Id == appointment.TimeSlotId && ts.Active == true).SingleOrDefault();
                    if (timeSlot == null) continue;
                }
                if (patient == null)
                {
                    patient = _context.Patients.Where(pt => pt.Id == appointment.PatientId && pt.Active == true).SingleOrDefault();
                    if (patient == null) continue;
                }
                if (vaccine == null)
                {
                    vaccine = _context.Vaccines.Where(vc => vc.Id == appointment.VaccineId && vc.Active == true).SingleOrDefault();
                    if (vaccine == null) continue;
                }
                if (timeSlot.Active == false || timeSlot.DoctorId != docId) continue;
                DoctorFormerAppointmentDTO doctorFormerAppointmentDTO = new DoctorFormerAppointmentDTO();
                doctorFormerAppointmentDTO.vaccineName = vaccine.Name;
                doctorFormerAppointmentDTO.vaccineCompany = vaccine.Company;
                doctorFormerAppointmentDTO.vaccineVirus = vaccine.Virus.ToString();
                doctorFormerAppointmentDTO.whichVaccineDose = appointment.WhichDose;
                doctorFormerAppointmentDTO.appointmentId = appointment.Id.ToString();
                doctorFormerAppointmentDTO.patientFirstName = patient.FirstName;
                doctorFormerAppointmentDTO.patientLastName = patient.LastName;
                doctorFormerAppointmentDTO.PESEL = patient.PESEL;
                doctorFormerAppointmentDTO.state = appointment.State.ToString();
                doctorFormerAppointmentDTO.batchNumber = appointment.VaccineBatchNumber;
                doctorFormerAppointmentDTO.from = timeSlot.From.ToString(_dateTimeFormat);
                doctorFormerAppointmentDTO.to = timeSlot.To.ToString(_dateTimeFormat);
                doctorFormerAppointmentDTO.certifyState = appointment.CertifyState.ToString();
                result.Add(doctorFormerAppointmentDTO);
            }
            return result;
        }
        [ProducesResponseType(typeof(IEnumerable<DoctorIncomingAppointmentDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("incomingAppointments/{doctorId}")]
        public ActionResult<IEnumerable<DoctorIncomingAppointmentDTO>> GetIncomingAppointments(string doctorId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<DoctorIncomingAppointmentDTO> result;
            try
            {
                result = fetchIncomingAppointments(doctorId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
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
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return null;

            List<DoctorIncomingAppointmentDTO> result = new List<DoctorIncomingAppointmentDTO>();
            var appointments = _context.Appointments.Where(ap => ap.State == AppointmentState.Planned).Include(ap => ap.TimeSlot).Include(ap => ap.Patient).Include(ap => ap.Vaccine).ToList();
            foreach (Appointment appointment in appointments)
            {
                TimeSlot timeSlot = appointment.TimeSlot;
                Patient patient = appointment.Patient;
                Vaccine vaccine = appointment.Vaccine;
                if (timeSlot == null)
                {
                    timeSlot = _context.TimeSlots.Where(ts => ts.Id == appointment.TimeSlotId && ts.Active == true).SingleOrDefault();
                    if (timeSlot == null) continue;
                }
                if (patient == null)
                {
                    patient = _context.Patients.Where(pt => pt.Id == appointment.PatientId && pt.Active == true).SingleOrDefault();
                    if (patient == null) continue;
                }
                if (vaccine == null)
                {
                    vaccine = _context.Vaccines.Where(vc => vc.Id == appointment.VaccineId && vc.Active == true).SingleOrDefault();
                    if (vaccine == null) continue;
                }
                if (timeSlot.Active == false || timeSlot.DoctorId != docId ||
                    patient.Active == false || vaccine.Active == false) continue;
                DoctorIncomingAppointmentDTO doctorFormerAppointmentDTO = new DoctorIncomingAppointmentDTO();
                doctorFormerAppointmentDTO.vaccineName = vaccine.Name;
                doctorFormerAppointmentDTO.vaccineCompany = vaccine.Company;
                doctorFormerAppointmentDTO.vaccineVirus = vaccine.Virus.ToString();
                doctorFormerAppointmentDTO.whichVaccineDose = appointment.WhichDose;
                doctorFormerAppointmentDTO.appointmentId = appointment.Id.ToString();
                doctorFormerAppointmentDTO.patientFirstName = patient.FirstName;
                doctorFormerAppointmentDTO.patientLastName = patient.LastName;
                doctorFormerAppointmentDTO.from = timeSlot.From.ToString(_dateTimeFormat);
                doctorFormerAppointmentDTO.to = timeSlot.To.ToString(_dateTimeFormat);
                result.Add(doctorFormerAppointmentDTO);
            }
            return result.AsEnumerable();
        }
        [ProducesResponseType(typeof(IEnumerable<DoctorMarkedAppointmentResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("vaccinate/{doctorId}/{appointmentId}")]
        public ActionResult<DoctorMarkedAppointmentResponseDTO> GetIncomingAppointment(string doctorId, string appointmentId)
        {
            // TODO: 401/403
            DoctorMarkedAppointmentResponseDTO result;
            try
            {
                result = fetchIncomingAppointment(doctorId, appointmentId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == null) return NotFound();
            return Ok(result);
        }
        private DoctorMarkedAppointmentResponseDTO fetchIncomingAppointment(string doctorId, string appointmentId)
        {
            Guid docId, apId;
            try
            {
                docId = Guid.Parse(doctorId);
                apId = Guid.Parse(appointmentId);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return null;

            var appointment = _context.Appointments.Where(ap => ap.Id == apId && ap.State == AppointmentState.Planned)
                .Include(ap => ap.Patient).Include(ap => ap.TimeSlot).Include(ap => ap.Vaccine).SingleOrDefault();
            if (appointment == null) return null;
            var patient = _context.Patients.Where(p => p.Id == appointment.PatientId).FirstOrDefault();
            var timeSlot = _context.TimeSlots.Where(ts => ts.Id == appointment.TimeSlotId).FirstOrDefault();
            var vaccine = _context.Vaccines.Where(v => v.Id == appointment.VaccineId).FirstOrDefault();
            if (patient.Active == false || timeSlot.Active == false ||
                vaccine.Active == false || timeSlot.DoctorId != docId) return null;
            DoctorMarkedAppointmentResponseDTO result = new DoctorMarkedAppointmentResponseDTO()
            {
                vaccineName = appointment.Vaccine.Name,
                vaccineCompany = appointment.Vaccine.Company,
                numberOfDoses = appointment.Vaccine.NumberOfDoses,
                minDaysBetweenDoses = appointment.Vaccine.MinDaysBetweenDoses,
                maxDaysBetweenDoses = appointment.Vaccine.MaxDaysBetweenDoses,
                virusName = appointment.Vaccine.Virus.ToString(),
                minPatientAge = appointment.Vaccine.MinPatientAge,
                maxPatientAge = appointment.Vaccine.MaxPatientAge,
                whichVaccineDose = appointment.WhichDose,
                patientFirstName = appointment.Patient.FirstName,
                patientLastName = appointment.Patient.LastName,
                PESEL = appointment.Patient.PESEL,
                dateOfBirth = appointment.Patient.DateOfBirth.ToString(_dateFormat),
                from = appointment.TimeSlot.From.ToString(_dateTimeFormat),
                to = appointment.TimeSlot.To.ToString(_dateTimeFormat),
            };
            return result;
        }
        [ProducesResponseType(typeof(IEnumerable<DoctorConfirmVaccinationResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("vaccinate/confirmVaccination/{doctorId}/{appointmentId}/{batchId}")]
        public ActionResult<DoctorConfirmVaccinationResponseDTO> ConfirmVaccination(string doctorId, string appointmentId, string batchId)
        {
            DoctorConfirmVaccinationResponseDTO result;
            try
            {
                result = tryConfirmVaccination(doctorId, appointmentId, batchId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == null) return NotFound();
            return Ok(result);
        }
        private DoctorConfirmVaccinationResponseDTO tryConfirmVaccination(string doctorId, string appointmentId, string batchId)
        {
            Guid docId, apId;
            try
            {
                docId = Guid.Parse(doctorId);
                apId = Guid.Parse(appointmentId);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            if (batchId == null) throw new BadRequestException();
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return null;

            var appointment = _context.Appointments.Where(ap => ap.Id == apId && ap.State == AppointmentState.Planned)
                .Include(ap => ap.Patient).Include(ap => ap.TimeSlot).Include(ap => ap.Vaccine).SingleOrDefault();
            if (appointment == null) return null;
            var patient = _context.Patients.Where(p => p.Id == appointment.PatientId).FirstOrDefault();
            var timeSlot = _context.TimeSlots.Where(ts => ts.Id == appointment.TimeSlotId).FirstOrDefault();
            var vaccine = _context.Vaccines.Where(v => v.Id == appointment.VaccineId).FirstOrDefault();
            if (timeSlot.Active == false ||
                vaccine.Active == false || timeSlot.DoctorId != docId) return null;

            DoctorConfirmVaccinationResponseDTO result = new DoctorConfirmVaccinationResponseDTO();
            if (appointment.WhichDose == appointment.Vaccine.NumberOfDoses) // That was the last dose for that vaccine
            {
                result.canCertify = true;
                appointment.CertifyState = CertifyState.LastNotCertified;
            }
            else
            {
                result.canCertify = false;
                appointment.CertifyState = CertifyState.NotLast;
            }

            appointment.State = AppointmentState.Finished;
            appointment.VaccineBatchNumber = batchId;
            _context.SaveChanges();
            if (_mailService != null)
            {
                MailRequest request = new MailRequest();
                request.Subject = "Visit completed";
                request.Body = "Your visit from " + timeSlot.From + " to " + timeSlot.To + " has just finished.";
                request.ToEmail = patient.Mail;
                try
                {
                    _mailService.SendEmailAsync(request);
                }
                catch
                {
                    throw;
                } 
            }
            return result;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("vaccinate/vaccinationDidNotHappen/{doctorId}/{appointmentId}")]
        public IActionResult VaccinationDidNotHappen(string doctorId, string appointmentId)
        {
            bool result;
            try
            {
                result = tryVaccinationDidNotHappen(doctorId, appointmentId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return NotFound();
            return Ok();
        }
        private bool tryVaccinationDidNotHappen(string doctorId, string appointmentId)
        {
            Guid docId, apId;
            try
            {
                docId = Guid.Parse(doctorId);
                apId = Guid.Parse(appointmentId);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return false;

            var appointment = _context.Appointments.Where(ap => ap.Id == apId && ap.State == AppointmentState.Planned)
                .Include(ap => ap.TimeSlot).Include(ap => ap.Vaccine).Include(ap => ap.Patient).SingleOrDefault();
            if (appointment == null) return false;
            var patient = _context.Patients.Where(p => p.Id == appointment.PatientId).FirstOrDefault();
            var timeSlot = _context.TimeSlots.Where(ts => ts.Id == appointment.TimeSlotId).FirstOrDefault();
            var vaccine = _context.Vaccines.Where(v => v.Id == appointment.VaccineId).FirstOrDefault();
            if (timeSlot.Active == false ||
                vaccine.Active == false || timeSlot.DoctorId != docId) return false;
            appointment.State = AppointmentState.Cancelled; // At least I assume so
            _context.SaveChanges();
            if (_mailService != null)
            {
                MailRequest request = new MailRequest();
                request.Subject = "You missed your visit";
                request.Body = "You have missed your visit from " + timeSlot.From + " to " + timeSlot.To + "! " +
                    "You need to book a new one now.";
                request.ToEmail = patient.Mail;
                try
                {
                    _mailService.SendEmailAsync(request);
                }
                catch
                {
                    throw;
                } 
            }
            return true;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("vaccinate/certify/{doctorId}/{appointmentId}")]
        public IActionResult Certify(string doctorId, string appointmentId)
        {
            bool result;
            try
            {
                result = tryCertify(doctorId, appointmentId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return NotFound();
            return Ok();
        }
        private bool tryCertify(string doctorId, string appointmentId)
        {
            Guid docId, apId;
            try
            {
                docId = Guid.Parse(doctorId);
                apId = Guid.Parse(appointmentId);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            var checkIfDoctorActive = _context.Doctors.Where(doc => doc.Id == docId && doc.Active == true).FirstOrDefault();
            if (checkIfDoctorActive == null) return false;

            var appointment = _context.Appointments.Where(ap => ap.Id == apId && ap.State == AppointmentState.Finished && ap.CertifyState == CertifyState.LastNotCertified
            && ap.VaccineBatchNumber != null).Include(ap => ap.TimeSlot).Include(ap => ap.Patient).Include(ap => ap.Vaccine).SingleOrDefault();
            if (appointment == null) return false;
            var patient = _context.Patients.Where(p => p.Id == appointment.PatientId).FirstOrDefault();
            var timeSlot = _context.TimeSlots.Where(ts => ts.Id == appointment.TimeSlotId).FirstOrDefault();
            var vaccine = _context.Vaccines.Where(v => v.Id == appointment.VaccineId).FirstOrDefault();
            if (timeSlot.Active == false || patient.Active == false ||
                vaccine.Active == false || timeSlot.DoctorId != docId) return false;
            Certificate newCert = new Certificate()
            {
                Id = Guid.NewGuid(),
                Patient = patient,
                PatientId = appointment.PatientId,
                Vaccine = vaccine,
                VaccineId = appointment.VaccineId,
                // Url = "randomFakeUrl", // to change to something proper once we get there
            };
            string url = GenerateCertificate(appointment, newCert.Id.ToString()).Result;
            newCert.Url = url;
            _context.Certificates.Add(newCert);
            appointment.CertifyState = CertifyState.Certified;
            _context.SaveChanges();
            if (_mailService != null)
            {
                MailRequest request = new MailRequest();
                request.Subject = "Certificate ready";
                request.Body = "You have just received a certificate for " + vaccine.Name +
                    " vaccine. Check it out now on our website!";
                request.ToEmail = patient.Mail;
                try
                {
                    _mailService.SendEmailAsync(request);
                }
                catch
                {
                    throw;
                }
            }
            return true;
        }

        private async Task<string> GenerateCertificate(Appointment appointment, string certifyGuid)
        {
            var patient = _context.Patients.SingleOrDefault(p => p.Id == appointment.PatientId);
            if (patient == null)
                throw new BadRequestException();
            var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
            if (timeSlot == null)
                throw new BadRequestException();
            var date = timeSlot.From.ToString(_dateFormat);
            var vaccine = _context.Vaccines.SingleOrDefault(v => v.Id == appointment.VaccineId);
            if (vaccine == null)
                throw new BadRequestException();

            //string urlPatient = patient.FirstName.Replace(" ", "%20") + "_" + patient.LastName.Replace(" ", "%20");
            string urlPatient = (patient.FirstName + "_" + patient.LastName).Replace('ą', 'a').Replace('Ą', 'A').Replace('ć', 'c').Replace('Ć', 'C').Replace('ę', 'e')
                .Replace('Ę', 'E').Replace('ł', 'l').Replace('Ł', 'L').Replace('ó', 'o').Replace('Ó', 'O').Replace('ń', 'n').Replace('Ń', 'N').Replace('ś', 's')
                .Replace('Ś', 'S').Replace('ź', 'z').Replace('Ź', 'Z').Replace('ż', 'z').Replace('Ż', 'Z');
            string pdfName = Guid.NewGuid().ToString() + ".pdf";
            string url = _storageUrlBase + urlPatient + "/" + pdfName;
            //string url = _storageUrlBase + urlPatient + "/";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont fontTitle = new XFont("Arial", 32, XFontStyle.Bold);

            gfx.DrawString("Certyfikat szczepienia", fontTitle, XBrushes.Black, new XRect(0, 0, page.Width, 3*page.Height/10), XStringFormats.Center);

            XFont fontField = new XFont("Arial", 8, XFontStyle.Regular);
            double fieldHeight = 10;
            XFont fontData = new XFont("Arial", 16, XFontStyle.Bold);
            double dataHeight = 20;
            double gapHeight = 40;
            XFont fontGuid = new XFont("Arial", 8, XFontStyle.Regular);

            gfx.DrawString("Nazwisko / Surname", fontField, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10, 4 * page.Width / 5, fieldHeight), XStringFormats.CenterLeft);
            gfx.DrawString(patient.LastName, fontData, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + fieldHeight, 4 * page.Width / 5, dataHeight), XStringFormats.CenterLeft);

            gfx.DrawString("Imiona / Names", fontField, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + gapHeight, 4 * page.Width / 5, fieldHeight), XStringFormats.CenterLeft);
            gfx.DrawString(patient.FirstName, fontData, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + gapHeight + fieldHeight, 4 * page.Width / 5, dataHeight), XStringFormats.CenterLeft);

            gfx.DrawString("Wirus / Virus", fontField, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 2 * gapHeight, 4 * page.Width / 5, fieldHeight), XStringFormats.CenterLeft);
            gfx.DrawString(vaccine.Virus.ToString(), fontData, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 2 * gapHeight + fieldHeight, 4 * page.Width / 5, dataHeight), XStringFormats.CenterLeft);

            gfx.DrawString("Szczepionka / Vaccine", fontField, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 3 * gapHeight, 4 * page.Width / 5, fieldHeight), XStringFormats.CenterLeft);
            gfx.DrawString(vaccine.Company + " " + vaccine.Name, fontData, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 3 * gapHeight + fieldHeight, 4 * page.Width / 5, dataHeight), XStringFormats.CenterLeft);

            gfx.DrawString("Dawka / Dose", fontField, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 4 * gapHeight, 4 * page.Width / 5, fieldHeight), XStringFormats.CenterLeft);
            gfx.DrawString($"{appointment.WhichDose}", fontData, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 4 * gapHeight + fieldHeight, 4 * page.Width / 5, dataHeight), XStringFormats.CenterLeft);

            gfx.DrawString("Data szczepienia / Vaccination date", fontField, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 5 * gapHeight, 4 * page.Width / 5, fieldHeight), XStringFormats.CenterLeft);
            gfx.DrawString(date, fontData, XBrushes.Black, new XRect(page.Width / 5, 3 * page.Height / 10 + 5 * gapHeight + fieldHeight, 4 * page.Width / 5, dataHeight), XStringFormats.CenterLeft);

            XBrush brush = new XSolidBrush(new XColor { A = 50, R = 128, G = 128, B = 128});
            gfx.DrawString(certifyGuid, fontGuid, brush, new XRect(0, page.Height - 42, page.Width, 42), XStringFormats.Center);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            PayloadGenerator.Url urlPayload = new PayloadGenerator.Url(url);
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(urlPayload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(6);

            using(MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                XImage image = XImage.FromStream(stream);

                gfx.DrawImage(image, new XPoint((page.Width - image.PointWidth) / 2, page.Height - 52 - image.PointHeight));
            }

            XFont watermarkFont = new XFont("Arial", 50, XFontStyle.Italic);
            var watermarkSize = gfx.MeasureString(_siteName, watermarkFont);

            gfx.TranslateTransform(page.Width / 2, page.Height / 2);
            gfx.RotateTransform(-Math.Atan(page.Height / page.Width) * 180 / Math.PI);
            gfx.TranslateTransform(-page.Width / 2, -page.Height / 2);

            var format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;

            brush = new XSolidBrush(XColor.FromArgb(50, 128, 128, 128));

            gfx.DrawString(_siteName, watermarkFont, brush, new XPoint((page.Width - watermarkSize.Width) / 2, (page.Height - watermarkSize.Height) / 2), format);

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);

                string connectionString = _configuration.GetConnectionString("AppStorage");
                string containerName = "certificates";
                var serviceClient = new BlobServiceClient(connectionString); // here
                var containerClient = serviceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(urlPatient + "/" + pdfName);
                await blobClient.UploadAsync(stream, true);
            }


            return url;

        }

        /*
        [AllowAnonymous]
        [HttpGet("test/{appointmentId}/{certifyGuid}")]
        public IActionResult Test(string appointmentId, string certifyGuid)
        {
            Guid apId;
            try
            {
                apId = Guid.Parse(appointmentId);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }

            var appointment = _context.Appointments.SingleOrDefault(a => a.Id == apId);

            if (appointment == null)
                return NotFound();

            try
            {
                GenerateCertificate(appointment, certifyGuid);
            }
            catch
            {
                return BadRequest();
            }
            
            return Ok();
        }*/
    }
}
