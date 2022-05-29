using EmailService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VaccinationSystem.Config;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.Errors;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.Models;

namespace VaccinationSystem.Controllers
{
    [Authorize(Policy = "PatientPolicy")]
    [ApiController]
    [Route("patient")]
    public class PatientController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;
        private readonly IMailService _mailService;
        private readonly string _dateTimeFormat = "dd-MM-yyyy HH\\:mm";
        private readonly string _dateFormat = "dd-MM-yyyy";
        public PatientController(VaccinationSystemDbContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        [ProducesResponseType(typeof(IEnumerable<PatientInfoResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("info/{patientId}")]
        public ActionResult<PatientInfoResponseDTO> GetPatientInfo(string patientId)
        {
            PatientInfoResponseDTO result;
            try
            {
                result = fetchPatientInfo(patientId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == null) return NotFound();
            return Ok(result);
        }
        private PatientInfoResponseDTO fetchPatientInfo(string patientId)
        {
            Guid patId;
            try
            {
                patId = Guid.Parse(patientId);
            }
            catch(ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            var patient = _context.Patients.Where(pat => pat.Id == patId && pat.Active == true).SingleOrDefault();
            if (patient == null) return null;
            PatientInfoResponseDTO result = new PatientInfoResponseDTO()
            {
                firstName = patient.FirstName,
                lastName = patient.LastName,
                PESEL = patient.PESEL,
                dateOfBirth = patient.DateOfBirth.Date.ToString(_dateFormat),
                mail = patient.Mail,
                phoneNumber = patient.PhoneNumber
            };
            return result;
        }
        [ProducesResponseType(typeof(IEnumerable<TimeSlotFilterResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("timeSlots/filter")]
        public ActionResult<IEnumerable<TimeSlotFilterResponseDTO>> FilterTimeSlots(string city, string dateFrom, string dateTo, string virus)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<TimeSlotFilterResponseDTO> result;
            try
            {
                string patId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (patId == null) return Unauthorized();
                result = fetchFilteredTimeSlots(city, dateFrom, dateTo, virus, patId);
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }

        [NonAction]
        public IEnumerable<TimeSlotFilterResponseDTO> fetchFilteredTimeSlots(string city, string dateFrom, string dateTo, string virus, string patId)
        {
            List<TimeSlotFilterResponseDTO> result = new List<TimeSlotFilterResponseDTO>();
            Guid patientId;
            DateTime From, To;
            try
            {
                From = DateTime.ParseExact(dateFrom, _dateFormat, null).Date;
                To = DateTime.ParseExact(dateTo, _dateFormat, null).Date;
                To = To.AddDays(1);
                patientId = Guid.Parse(patId);
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            if (city == null || virus == null) throw new BadRequestException();
            List<TimeSlot> timeSlots;

            // Check if the patient already has a booked visit for this virus
            /*var booked = _context.Appointments.Include(ap => ap.Vaccine).Where(ap => ap.PatientId == patientId && ap.Vaccine.Virus.ToString() == virus &&
                ap.State == AppointmentState.Planned).FirstOrDefault();*/
            var beforeBooked = _context.Appointments.Where(ap => ap.PatientId == patientId && 
                ap.State == AppointmentState.Planned).Include(ap => ap.Vaccine).ToList();
            var booked = beforeBooked.Where(ap => ap.Vaccine.Virus.ToString() == virus).FirstOrDefault();
            if (booked != null) return null; // Patient already has a planned visit for this virus, he can't order a new one
            // Check if the patient was already vaccinated with a dose of a vaccine
            /*var vaccinated = _context.Appointments.Include(ap => ap.Vaccine).Include(ap => ap.TimeSlot).
                Where(ap => ap.PatientId == patientId && ap.Vaccine.Virus.ToString() == virus && ap.State == AppointmentState.Finished).ToList();*/
            var beforeVaccinated = _context.Appointments.Where(ap => ap.PatientId == patientId && ap.State == AppointmentState.Finished)
                .Include(ap => ap.Vaccine).Include(ap => ap.TimeSlot).ToList();
            var vaccinated = beforeVaccinated.Where(ap => ap.Vaccine.Virus.ToString() == virus).ToList();
            Vaccine vaccineToBeUsed = null;
            if (vaccinated != null && vaccinated.Count > 0) // He did, new time slots must be only for that type of vaccine
            {
                // get last date when patient can be vaccinated
                vaccinated.OrderBy(a => a.TimeSlot.To);
                vaccineToBeUsed = vaccinated.Last().Vaccine;
                DateTime minDate, maxDate;
                // Those can be changed, depending on whether it would be easier if they were on midnight for example
                if (vaccineToBeUsed.MinDaysBetweenDoses != -1) minDate = vaccinated.Last().TimeSlot.To.AddDays(vaccineToBeUsed.MinDaysBetweenDoses);
                else minDate = DateTime.Now;
                if (vaccineToBeUsed.MaxDaysBetweenDoses != -1) maxDate = vaccinated.Last().TimeSlot.To.AddDays(vaccineToBeUsed.MaxDaysBetweenDoses);
                else maxDate = DateTime.Now.AddDays(365);

                timeSlots = _context.TimeSlots.Where(timeSlot => timeSlot.Active == true && timeSlot.IsFree == true &&
                timeSlot.From >= From && timeSlot.From >= minDate && timeSlot.From <= maxDate && timeSlot.To <= To)
                    .Include(timeSlot => timeSlot.Doctor).ToList();
            }

            else
            {
            timeSlots = _context.TimeSlots.Where(timeSlot => timeSlot.Active == true && timeSlot.IsFree == true &&
                timeSlot.From >= From && timeSlot.To <= To).Include(timeSlot => timeSlot.Doctor).ToList();
            }
            foreach (TimeSlot timeSlot in timeSlots)
            {
                Patient patientAccount = _context.Patients.Where(patient => patient.Id == timeSlot.Doctor.PatientId).SingleOrDefault();
                TimeSlotFilterResponseDTO timeSlotFilterResponseDTO = new TimeSlotFilterResponseDTO();
                VaccinationCenter vaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Id == timeSlot.Doctor.VaccinationCenterId && vc.City == city).SingleOrDefault();
                if (vaccinationCenter == null) continue;
                List<OpeningHours> openingHours;
                List<VaccinesInVaccinationCenter> vaccineIDs;
                try
                {
                    openingHours = _context.OpeningHours.Where(oh => oh.VaccinationCenterId == vaccinationCenter.Id).ToList();
                    if (openingHours.Count == 0) continue;

                    if (vaccineToBeUsed != null)
                    {
                        vaccineIDs = _context.VaccinesInVaccinationCenter.Where
                            (vivc => vivc.VaccinationCenterId == vaccinationCenter.Id && vivc.VaccineId == vaccineToBeUsed.Id).ToList();
                    }

                    else
                    {
                        vaccineIDs = _context.VaccinesInVaccinationCenter
                            .Where(vivc => vivc.VaccinationCenterId == vaccinationCenter.Id).ToList();
                    }
                }
                catch(ArgumentNullException)
                {
                    continue;
                }
                if (vaccineIDs.Count == 0) continue;
                List<SimplifiedVaccineDTO> vaccines = new List<SimplifiedVaccineDTO>();
                bool foundVirus = false;
                foreach(VaccinesInVaccinationCenter vaccineID in vaccineIDs)
                {

                    if (vaccineToBeUsed != null && vaccineToBeUsed.Id != vaccineID.VaccineId) continue;

                    var vaccine = _context.Vaccines.Where(vac => vac.Id == vaccineID.VaccineId && vac.Active == true).SingleOrDefault();
                    if (vaccine == null) continue;
                    vaccines.Add(new SimplifiedVaccineDTO()
                    {
                        vaccineId = vaccine.Id.ToString(),
                        company = vaccine.Company,
                        name = vaccine.Name,
                        numberOfDoses = vaccine.NumberOfDoses,
                        minDaysBetweenDoses = vaccine.MinDaysBetweenDoses,
                        maxDaysBetweenDoses = vaccine.MaxDaysBetweenDoses,
                        virus = vaccine.Virus.ToString(),
                        minPatientAge = vaccine.MinPatientAge,
                        maxPatientAge = vaccine.MaxPatientAge,
                    });
                    if (vaccine.Virus.ToString() == virus) foundVirus = true;
                }
                if (!foundVirus) continue;
                if (vaccines.Count == 0) continue;
                openingHours.Sort((p1, p2) => p1.WeekDay.CompareTo(p2.WeekDay));
                List<OpeningHoursDayDTO> openingHoursDTOs = new List<OpeningHoursDayDTO>();
                foreach(OpeningHours oh in openingHours)
                {
                    openingHoursDTOs.Add(new OpeningHoursDayDTO()
                    {
                        from = oh.From.ToString(),
                        to = oh.To.ToString(),
                    });
                }

                timeSlotFilterResponseDTO.timeSlotId = timeSlot.Id.ToString();
                timeSlotFilterResponseDTO.from = timeSlot.From.ToString(_dateTimeFormat);
                timeSlotFilterResponseDTO.to = timeSlot.To.ToString(_dateTimeFormat);
                timeSlotFilterResponseDTO.vaccinationCenterName = vaccinationCenter.Name;
                timeSlotFilterResponseDTO.vaccinationCenterCity = vaccinationCenter.City;
                timeSlotFilterResponseDTO.vaccinationCenterStreet = vaccinationCenter.Address;
                timeSlotFilterResponseDTO.availableVaccines = vaccines;
                timeSlotFilterResponseDTO.openingHours = openingHoursDTOs;
                timeSlotFilterResponseDTO.doctorFirstName = patientAccount.FirstName;
                timeSlotFilterResponseDTO.doctorLastName = patientAccount.LastName;
                result.Add(timeSlotFilterResponseDTO);
            }
            return result;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("timeSlots/book/{patientId}/{timeSlotId}/{vaccineId}")]
        public IActionResult BookVisit(string patientId, string timeSlotId, string vaccineId)
        {
            // TODO: Token verification for 401 and 403 error codes
            bool result;
            try
            {
                result = tryBookVisit(patientId, timeSlotId, vaccineId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return NotFound();
            return Ok();
        }
        private bool tryBookVisit(string patientId, string timeSlotId, string vaccineId)
        {
            Guid patId, tsId, vacId;
            try
            {
                patId = Guid.Parse(patientId);
                tsId = Guid.Parse(timeSlotId);
                vacId = Guid.Parse(vaccineId);
            }
            catch(ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            var vaccine = _context.Vaccines.Where(vac => vac.Id == vacId && vac.Active == true).SingleOrDefault();
            if (vaccine == null) return false;
            var patient = _context.Patients.Where(patient => patient.Id == patId && patient.Active == true).SingleOrDefault();
            if (patient == null) return false;
            var timeSlot = _context.TimeSlots.Where(ts => ts.Id == tsId && ts.Active == true && ts.IsFree == true).SingleOrDefault();
            if (timeSlot == null) return false;
            int whichDose = _context.Appointments.Where(ap => ap.PatientId == patId && ap.VaccineId == vacId && ap.State == AppointmentState.Finished).Count();
            whichDose++;
            var virus = vaccine.Virus;
            var vaccinatedAgainst = _context.Appointments.Where(ap => ap.PatientId == patId && ap.State == AppointmentState.Finished).Include(ap => ap.Vaccine);
            int differentVaccines = vaccinatedAgainst.Where(ap => ap.Vaccine.Virus == virus && ap.VaccineId != vacId).Count();
            if (differentVaccines > 0 || whichDose > vaccine.NumberOfDoses) return false; // TODO: add checking for minimum/maximum days between doses
            Appointment appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                WhichDose = whichDose,
                TimeSlotId = tsId,
                TimeSlot = timeSlot,
                PatientId = patId,
                Patient = patient,
                VaccineId = vacId,
                Vaccine = vaccine,
                State = AppointmentState.Planned
            };
            _context.Appointments.Add(appointment);
            timeSlot.IsFree = false;
            _context.SaveChanges();
            if(_mailService != null)
            {
                MailRequest request = new MailRequest();
                request.Subject = "Visit booking successful";
                request.Body = "You have successfully booked a visit!";
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
        [ProducesResponseType(typeof(IEnumerable<FutureAppointmentDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("appointments/incomingAppointments/{patientId}")]
        public ActionResult<IEnumerable<FutureAppointmentDTO>> GetIncomingVisits(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<FutureAppointmentDTO> result;
            try
            {
                result = fetchIncomingVisits(patientId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<FutureAppointmentDTO> fetchIncomingVisits(string patientId)
        {
            List<FutureAppointmentDTO> result = new List<FutureAppointmentDTO>();
            Guid patId;
            try
            {
                patId = Guid.Parse(patientId);
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfPatientIsActive = _context.Patients.Where(pat => pat.Id == patId && pat.Active == true).FirstOrDefault();
            if (checkIfPatientIsActive == null) return null;
            var appointments = _context.Appointments.Where(ap => ap.PatientId == patId && ap.State == Models.AppointmentState.Planned).Include(ap => ap.TimeSlot).Include(ap => ap.Vaccine).ToList();
            foreach(Appointment appointment in appointments)
            {
                FutureAppointmentDTO futureAppointmentDTO = new FutureAppointmentDTO();
                Doctor doctor = _context.Doctors.Where(doc => doc.Id == appointment.TimeSlot.DoctorId && doc.Active == true).SingleOrDefault();
                if (doctor == null) continue;
                Patient doctorPatientAccount = _context.Patients.Where(pat => pat.Id == doctor.PatientId && pat.Active == true).SingleOrDefault();
                if (doctorPatientAccount == null) continue;
                VaccinationCenter vaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Id == doctor.VaccinationCenterId && vc.Active == true).SingleOrDefault();
                if (vaccinationCenter == null) continue;

                futureAppointmentDTO.vaccineName = appointment.Vaccine.Name;
                futureAppointmentDTO.vaccineCompany = appointment.Vaccine.Company;
                futureAppointmentDTO.vaccineVirus = appointment.Vaccine.Virus.ToString();
                futureAppointmentDTO.whichVaccineDose = appointment.WhichDose;
                futureAppointmentDTO.appointmentId = appointment.Id.ToString();
                futureAppointmentDTO.windowBegin = appointment.TimeSlot.From.ToString(_dateTimeFormat);
                futureAppointmentDTO.windowEnd = appointment.TimeSlot.To.ToString(_dateTimeFormat);
                futureAppointmentDTO.vaccinationCenterName = vaccinationCenter.Name;
                futureAppointmentDTO.vaccinationCenterCity = vaccinationCenter.City;
                futureAppointmentDTO.vaccinationCenterStreet = vaccinationCenter.Address;
                futureAppointmentDTO.doctorFirstName = doctorPatientAccount.FirstName;
                futureAppointmentDTO.doctorLastName = doctorPatientAccount.LastName;
                result.Add(futureAppointmentDTO);
            }
            return result;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("appointments/incomingAppointments/cancelAppointments/{patientId}/{appointmentId}")]
        public IActionResult CancelVisit(string appointmentId, string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            bool result;
            try
            {
                result = modifyCancelVisit(appointmentId, patientId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == false) return NotFound();
            return Ok();
        }
        private bool modifyCancelVisit(string appointmentId, string patientId)
        {
            Guid appId, patId;
            try
            {
                appId = Guid.Parse(appointmentId);
                patId = Guid.Parse(patientId);
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfPatientIsActive = _context.Patients.Where(pat => pat.Id == patId && pat.Active == true).FirstOrDefault();
            if (checkIfPatientIsActive == null) return false;
            var appointment = _context.Appointments.Where(a => a.Id == appId && a.PatientId == patId).FirstOrDefault();
            if (appointment == null || appointment.State != AppointmentState.Planned) return false;
            Guid timeSlotId = appointment.TimeSlotId.GetValueOrDefault();
            if (timeSlotId == null) return false;
            var timeSlot = _context.TimeSlots.SingleOrDefault(a => a.Id == timeSlotId);
            if (timeSlot == null) return false;
            appointment.State = Models.AppointmentState.Cancelled;
            timeSlot.IsFree = true;
            this._context.SaveChanges();
            if (_mailService != null)
            {
                MailRequest request = new MailRequest();
                request.Subject = "Visit cancellation successful";
                request.Body = "You have successfully cancelled a visit!";
                request.ToEmail = checkIfPatientIsActive.Mail;
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
        [ProducesResponseType(typeof(IEnumerable<FormerAppointmentDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("appointments/formerAppointments/{patientId}")]
        public ActionResult<IEnumerable<FormerAppointmentDTO>> GetFormerVisits(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<FormerAppointmentDTO> result;
            try
            {
                result = fetchFormerVisits(patientId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<FormerAppointmentDTO> fetchFormerVisits(string patientId)
        {
            List<FormerAppointmentDTO> result = new List<FormerAppointmentDTO>();
            Guid patId;
            try
            {
                patId = Guid.Parse(patientId);
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfPatientIsActive = _context.Patients.Where(pat => pat.Id == patId && pat.Active == true).FirstOrDefault();
            if (checkIfPatientIsActive == null) return null;
            var appointments = _context.Appointments.Where(ap => ap.PatientId == patId && ap.State != Models.AppointmentState.Planned).Include(ap => ap.TimeSlot)
                .Include(ap => ap.Vaccine).ToList();
            foreach(Appointment appointment in appointments)
            {
                FormerAppointmentDTO formerAppointmentDTO = new FormerAppointmentDTO();
                Doctor doctor = _context.Doctors.Where(doc => doc.Id == appointment.TimeSlot.DoctorId).SingleOrDefault();
                if (doctor == null) continue;
                Patient doctorPatientAccount = _context.Patients.Where(pat => pat.Id == doctor.PatientId).SingleOrDefault();
                if (doctorPatientAccount == null) continue;
                VaccinationCenter vaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Id == doctor.VaccinationCenterId).SingleOrDefault();
                if (vaccinationCenter == null) continue;

                formerAppointmentDTO.vaccineName = appointment.Vaccine.Name;
                formerAppointmentDTO.vaccineCompany = appointment.Vaccine.Company;
                formerAppointmentDTO.vaccineVirus = appointment.Vaccine.Virus.ToString();
                formerAppointmentDTO.whichVaccineDose = appointment.WhichDose;
                formerAppointmentDTO.appointmentId = appointment.Id.ToString();
                formerAppointmentDTO.windowBegin = appointment.TimeSlot.From.ToString(_dateTimeFormat);
                formerAppointmentDTO.windowEnd = appointment.TimeSlot.To.ToString(_dateTimeFormat);
                formerAppointmentDTO.vaccinationCenterName = vaccinationCenter.Name;
                formerAppointmentDTO.vaccinationCenterCity = vaccinationCenter.City;
                formerAppointmentDTO.vaccinationCenterStreet = vaccinationCenter.Address;
                formerAppointmentDTO.doctorFirstName = doctorPatientAccount.FirstName;
                formerAppointmentDTO.doctorLastName = doctorPatientAccount.LastName;
                formerAppointmentDTO.visitState = appointment.State.ToString();
                if (appointment.Vaccine.NumberOfDoses == appointment.WhichDose)
                {
                    formerAppointmentDTO.certifyState = CertifyState.LastNotCertified.ToString();
                }
                else formerAppointmentDTO.certifyState = CertifyState.NotLast.ToString();

                result.Add(formerAppointmentDTO);
            }
            return result;
        }
        [ProducesResponseType(typeof(IEnumerable<BasicCertificateInfoDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("certificates/{patientId}")]
        public ActionResult<IEnumerable<BasicCertificateInfoDTO>> GetCertificates(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            IEnumerable<BasicCertificateInfoDTO> result;
            try
            {
                result = fetchCertificates(patientId);
            }
            catch(BadRequestException)
            {
                return BadRequest();
            }
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<BasicCertificateInfoDTO> fetchCertificates(string patientId)
        {
            Guid patId;
            try
            {
                patId = Guid.Parse(patientId);
            }
            catch(FormatException)
            {
                throw new BadRequestException();
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            var checkIfPatientIsActive = _context.Patients.Where(pat => pat.Id == patId && pat.Active == true).FirstOrDefault();
            if (checkIfPatientIsActive == null) return null;
            List<BasicCertificateInfoDTO> result = new List<BasicCertificateInfoDTO>();
            var certificates = _context.Certificates.Where(c => c.PatientId == patId).Include(c => c.Vaccine).ToList();
            foreach(Certificate certificate in certificates)
            {
                result.Add(new BasicCertificateInfoDTO()
                {
                    url = certificate.Url,
                    vaccineName = certificate.Vaccine.Name,
                    vaccineCompany = certificate.Vaccine.Company,
                    virus = certificate.Vaccine.Virus.ToString(),
                });
            }
            return result;
        }
    }
}
