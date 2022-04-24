using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Config;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.Models;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("patient")]
    public class PatientController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;
        private readonly string _dateTimeFormat = "dd-MM-yyyy HH\\:mm";
        public PatientController(VaccinationSystemDbContext context)
        {
            _context = context;
        }

        [HttpGet("timeSlots/Filter")]
        public ActionResult<IEnumerable<TimeSlotFilterResponseDTO>> FilterTimeSlots(string city, string dateFrom, string dateTo, string virus)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = fetchFilteredTimeSlots(city, dateFrom, dateTo, virus);
            if (result == null || result.Count() == 0) return NotFound();
            return Ok(result);
        }
        private IEnumerable<TimeSlotFilterResponseDTO> fetchFilteredTimeSlots(string city, string dateFrom, string dateTo, string virus)
        {
            List<TimeSlotFilterResponseDTO> result = new List<TimeSlotFilterResponseDTO>();
            DateTime From, To;
            try
            {
                //From = DateTime.Parse(dateFrom);
                From = DateTime.ParseExact(dateFrom, _dateTimeFormat, null);
                //To = DateTime.Parse(dateTo);
                To = DateTime.ParseExact(dateTo, _dateTimeFormat, null);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            var timeSlots = _context.TimeSlots.Where(timeSlot => timeSlot.Active == true && timeSlot.IsFree == true && timeSlot.From >= From && timeSlot.To <= To).Include(timeSlot => timeSlot.Doctor).ToList();
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
                    vaccineIDs = _context.VaccinesInVaccinationCenter.Where(vivc => vivc.VaccinationCenterId == vaccinationCenter.Id).ToList();
                }
                catch(ArgumentNullException)
                {
                    return null;
                }
                if (vaccineIDs.Count == 0) continue;
                List<SimplifiedVaccineDTO> vaccines = new List<SimplifiedVaccineDTO>();
                bool foundVirus = false;
                foreach(VaccinesInVaccinationCenter vaccineID in vaccineIDs)
                {
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
                        From = oh.From.ToString(),
                        To = oh.To.ToString(),
                    });
                }

                timeSlotFilterResponseDTO.TimeSlotId = timeSlot.Id.ToString();
                timeSlotFilterResponseDTO.From = timeSlot.From.ToString(_dateTimeFormat);
                timeSlotFilterResponseDTO.To = timeSlot.To.ToString(_dateTimeFormat);
                timeSlotFilterResponseDTO.VaccinationCenterName = vaccinationCenter.Name;
                timeSlotFilterResponseDTO.VaccinationCenterCity = vaccinationCenter.City;
                timeSlotFilterResponseDTO.VaccinationCenterStreet = vaccinationCenter.Address;
                timeSlotFilterResponseDTO.AvailableVaccines = vaccines;
                timeSlotFilterResponseDTO.OpeningHours = openingHoursDTOs;
                timeSlotFilterResponseDTO.DoctorFirstName = patientAccount.FirstName;
                timeSlotFilterResponseDTO.DoctorLastName = patientAccount.LastName;
                result.Add(timeSlotFilterResponseDTO);
            }
            return result;
        }

        [HttpPost("timeSlots/Book/{patientId}/{timeSlotId}/{vaccineId}")]
        public IActionResult BookVisit(string patientId, string timeSlotId, string vaccineId)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (!tryBookVisit(patientId, timeSlotId, vaccineId)) return NotFound();
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
                return false;
            }
            catch(FormatException)
            {
                return false;
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
            return true;
        }

        [HttpGet("appointments/incomingAppointments/{patientId}")]
        public ActionResult<IEnumerable<FutureAppointmentDTO>> GetIncomingVisits(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = fetchIncomingVisits(patientId);
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
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
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

                futureAppointmentDTO.VaccineName = appointment.Vaccine.Name;
                futureAppointmentDTO.VaccineCompany = appointment.Vaccine.Company;
                futureAppointmentDTO.VaccineVirus = appointment.Vaccine.Virus.ToString();
                futureAppointmentDTO.WhichVaccineDose = appointment.WhichDose;
                futureAppointmentDTO.AppointmentId = appointment.Id.ToString();
                futureAppointmentDTO.WindowBegin = appointment.TimeSlot.From.ToString(_dateTimeFormat);
                futureAppointmentDTO.WindowEnd = appointment.TimeSlot.To.ToString(_dateTimeFormat);
                futureAppointmentDTO.VaccinationCenterName = vaccinationCenter.Name;
                futureAppointmentDTO.VaccinationCenterCity = vaccinationCenter.City;
                futureAppointmentDTO.VaccinationCenterStreet = vaccinationCenter.Address;
                futureAppointmentDTO.DoctorFirstName = doctorPatientAccount.FirstName;
                futureAppointmentDTO.DoctorLastName = doctorPatientAccount.LastName;
                result.Add(futureAppointmentDTO);
            }
            return result;
        }

        [HttpDelete("appointments/incomingAppointments/cancelAppointments/{patientId}/{appointmentId}")]
        public IActionResult CancelVisit(string appointmentId, string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            if (!modifyCancelVisit(appointmentId, patientId)) return NotFound();
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
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            var appointment = _context.Appointments.Where(a => a.Id == appId && a.PatientId == patId).FirstOrDefault();
            if (appointment == null || appointment.State != AppointmentState.Planned) return false;
            Guid timeSlotId = appointment.TimeSlotId.GetValueOrDefault();
            if (timeSlotId == null) return false;
            var timeSlot = _context.TimeSlots.SingleOrDefault(a => a.Id == timeSlotId);
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
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
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

                formerAppointmentDTO.VaccineName = appointment.Vaccine.Name;
                formerAppointmentDTO.VaccineCompany = appointment.Vaccine.Company;
                formerAppointmentDTO.VaccineVirus = appointment.Vaccine.Virus.ToString();
                formerAppointmentDTO.WhichVaccineDose = appointment.WhichDose;
                formerAppointmentDTO.AppointmentId = appointment.Id.ToString();
                formerAppointmentDTO.WindowBegin = appointment.TimeSlot.From.ToString(_dateTimeFormat);
                formerAppointmentDTO.WindowEnd = appointment.TimeSlot.To.ToString(_dateTimeFormat);
                formerAppointmentDTO.VaccinationCenterName = vaccinationCenter.Name;
                formerAppointmentDTO.VaccinationCenterCity = vaccinationCenter.City;
                formerAppointmentDTO.VaccinationCenterStreet = vaccinationCenter.Address;
                formerAppointmentDTO.DoctorFirstName = doctorPatientAccount.FirstName;
                formerAppointmentDTO.DoctorLastName = doctorPatientAccount.LastName;
                formerAppointmentDTO.VisitState = appointment.State.ToString();

                result.Add(formerAppointmentDTO);
            }
            return result;
        }

        [HttpGet("certificates/{patientId}")]
        public ActionResult<IEnumerable<BasicCertificateInfoDTO>> GetCertificates(string patientId)
        {
            // TODO: Token verification for 401 and 403 error codes
            var result = fetchCertificates(patientId);
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
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            List<BasicCertificateInfoDTO> result = new List<BasicCertificateInfoDTO>();
            var certificates = _context.Certificates.Where(c => c.PatientId == patId).Include(c => c.Vaccine).ToList();
            foreach(Certificate certificate in certificates)
            {
                result.Add(new BasicCertificateInfoDTO()
                {
                    Url = certificate.Url,
                    VaccineName = certificate.Vaccine.Name,
                    VaccineCompany = certificate.Vaccine.Company,
                    Virus = certificate.Vaccine.Virus.ToString(),
                });
            }
            return result;
        }
    }
}
