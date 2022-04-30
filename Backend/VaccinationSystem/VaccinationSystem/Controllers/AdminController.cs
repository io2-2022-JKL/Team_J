using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.DTO.AdminDTOs;
using VaccinationSystem.Config;
using VaccinationSystem.Models;
using System.Diagnostics;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;
        private readonly string _dateFormat = "dd-MM-yyyy";
        private readonly string _dateTimeFormat = "dd-MM-yyyy HH\\:mm";
        private readonly string _timeSpanFormat = "hh\\:mm";

        public AdminController(VaccinationSystemDbContext context)
        {
            _context = context;
        }

        [HttpGet("patients")]
        public ActionResult<IEnumerable<PatientDTO>> GetPatients()
        {
            var result = GetAllPatients();
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        private IEnumerable<PatientDTO> GetAllPatients()
        {
            List<PatientDTO> result = new List<PatientDTO>();
            foreach(Patient patient in _context.Patients.ToList())
            {
                PatientDTO patientDTO = new PatientDTO();
                patientDTO.id = patient.Id.ToString();
                patientDTO.PESEL = patient.PESEL;
                patientDTO.firstName = patient.FirstName;
                patientDTO.lastName = patient.LastName;
                patientDTO.mail = patient.Mail;
                try
                {
                    patientDTO.dateOfBirth = patient.DateOfBirth.ToString(_dateFormat);
                }
                catch(FormatException)
                {
                    return null;
                }
                patientDTO.phoneNumber = patient.PhoneNumber;
                patientDTO.active = patient.Active;
                result.Add(patientDTO);
            }
            return result;
        }

        [HttpPost("patients/editPatient")]
        public IActionResult EditPatient(PatientDTO patientDTO)
        {
            return NotFound();
        }

        [HttpDelete("patients/deletePatient/{patientId}")]
        public IActionResult DeletePatient(string patientId)
        {
            var result = FindAndDeletePatient(patientId);
            return result;
        }

        private IActionResult FindAndDeletePatient(string patientId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(patientId);
            }
            catch(FormatException)
            {
                return NotFound();
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }
            Patient patient = _context.Patients.Where(patient => patient.Active == true && patient.Id == id).FirstOrDefault();
            if(patient != null)
            {
                Doctor doctor = _context.Doctors.Where(doc => doc.Active == true && doc.PatientAccount.Id == id).FirstOrDefault();
                if(doctor != null)
                {
                    doctor.Active = false;

                    foreach(var appointment in _context.Appointments.Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).Include("TimeSlots").ToList())
                    {
                        appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                        var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
                        if (timeSlot == null)
                            return NotFound();
                        appointment.TimeSlot = timeSlot;
                        appointment.TimeSlot.Active = false;
                    }
                    _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == true).ToList().ForEach(slot => { slot.Active = false; });

                }
                patient.Active = false;
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
            
        }

        [HttpGet("doctors")]
        public ActionResult<IEnumerable<GetDoctorsResponseDTO>> GetDoctors()
        {
            var result = GetAllDoctors();
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        private IEnumerable<GetDoctorsResponseDTO> GetAllDoctors()
        {
            List<GetDoctorsResponseDTO> result = new List<GetDoctorsResponseDTO>();
            foreach(Doctor doc in _context.Doctors.Include(doc => doc.PatientAccount).ToList())
            {
                var patientAccount = _context.Patients.SingleOrDefault(p => p.Id == doc.PatientId);
                if(patientAccount == null)
                {
                    return null;
                }
                doc.PatientAccount = patientAccount;
                GetDoctorsResponseDTO getDoctorsResponseDTO = new GetDoctorsResponseDTO();
                getDoctorsResponseDTO.id = doc.Id.ToString();
                getDoctorsResponseDTO.PESEL = doc.PatientAccount.PESEL;
                getDoctorsResponseDTO.firstName = doc.PatientAccount.FirstName;
                getDoctorsResponseDTO.lastName = doc.PatientAccount.LastName;
                getDoctorsResponseDTO.mail = doc.PatientAccount.Mail;
                try
                {
                    getDoctorsResponseDTO.dateOfBirth = doc.PatientAccount.DateOfBirth.ToString(_dateFormat);
                }
                catch(FormatException)
                {
                    return null;
                }
                getDoctorsResponseDTO.phoneNumber = doc.PatientAccount.PhoneNumber;
                getDoctorsResponseDTO.active = doc.Active;
                var vaccinationCenter = _context.VaccinationCenters.SingleOrDefault(vc => vc.Id == doc.VaccinationCenterId);
                if(vaccinationCenter == null)
                {
                    return null;
                }
                doc.VaccinationCenter = vaccinationCenter;
                getDoctorsResponseDTO.vaccinationCenterId = doc.VaccinationCenterId.ToString();
                getDoctorsResponseDTO.name = doc.VaccinationCenter.Name;
                getDoctorsResponseDTO.city = doc.VaccinationCenter.City;
                getDoctorsResponseDTO.street = doc.VaccinationCenter.Address;
                result.Add(getDoctorsResponseDTO);
            }
            return result;
        }

        [HttpPost("doctors/editDoctor")]
        public IActionResult EditDoctor(EditDoctorRequestDTO editDoctorRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("doctors/addDoctor")]
        public IActionResult AddDoctor(AddDoctorRequestDTO addDoctorRequestDTO)
        {
            var result = AddNewDoctor(addDoctorRequestDTO);
            return result;
        }

        private IActionResult AddNewDoctor(AddDoctorRequestDTO addDoctorRequestDTO)
        {
            Guid id;
            try
            {
                id = Guid.Parse(addDoctorRequestDTO.patientId);
            }
            catch (FormatException)
            {
                return NotFound();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            Doctor doctor = new Doctor();
            doctor.Id = Guid.NewGuid();
            doctor.PatientId = id;
            doctor.PatientAccount = _context.Patients.Where(patient => patient.Active == true && patient.Id == id).FirstOrDefault();
            if(doctor.PatientAccount == null)
            {
                return NotFound();
            }
            if(_context.Doctors.Where(doc => doc.PatientId == id && doc.Active == true).Any())
            {
                return NotFound();
            }
            Guid vcId;
            try
            {
                vcId = Guid.Parse(addDoctorRequestDTO.vaccinationCenterId);
            }
            catch (FormatException)
            {
                return NotFound();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            doctor.VaccinationCenterId = vcId;
            doctor.VaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Active == true && vc.Id == vcId).FirstOrDefault();
            if(doctor.VaccinationCenter == null)
            {
                return NotFound();
            }

            doctor.Active = true;

            _context.Doctors.Add(doctor);
            Debug.WriteLine(_context.SaveChanges());
            return Ok();

        }

        [HttpDelete("doctors/deleteDoctor/{doctorId}")]
        public IActionResult DeleteDoctor(string doctorId)
        {
            var result = FindAndDeleteDoctor(doctorId);
            return result;
        }

        private IActionResult FindAndDeleteDoctor(string doctorId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(doctorId);
            }
            catch (FormatException)
            {
                return NotFound();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }

            Doctor doctor = _context.Doctors.Where(doc => doc.Active == true && doc.Id == id).SingleOrDefault();
            if(doctor != null)
            {

                foreach(var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).ToList())
                {
                    appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                    var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
                    if(timeSlot == null)
                    {
                        return NotFound();
                    }
                    appointment.TimeSlot = timeSlot;
                    appointment.TimeSlot.Active = false;
                }
                _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == true).ToList().ForEach(slot => { slot.Active = false; }) ;

                doctor.Active = false;

                _context.SaveChanges();
                return Ok();
            }
            return NotFound();

        }

        [HttpGet("doctors/timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<TimeSlotDTO>> GetTimeSlots(string doctorId)
        {
            var result = GetAllDoctorTimeSlots(doctorId);
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        private IEnumerable<TimeSlotDTO> GetAllDoctorTimeSlots(string doctorId)
        {
            List<TimeSlotDTO> timeSlots = new List<TimeSlotDTO>();
            Guid id;
            try
            {
                id = Guid.Parse(doctorId);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            if (_context.Doctors.SingleOrDefault(doc => doc.Id == id) == null)
            {
                return null;
            }
            foreach (TimeSlot timeSlot in _context.TimeSlots.Where(ts => ts.DoctorId == id).ToList())
            {
                TimeSlotDTO timeSlotDTO = new TimeSlotDTO();
                timeSlotDTO.id = timeSlot.Id.ToString();
                try
                {
                    timeSlotDTO.from = timeSlot.From.ToString(_dateTimeFormat);
                    timeSlotDTO.to = timeSlot.To.ToString(_dateTimeFormat);
                }
                catch (FormatException)
                {
                    return null;
                }
                timeSlotDTO.isFree = timeSlot.IsFree;
                timeSlotDTO.active = timeSlot.Active;
                timeSlots.Add(timeSlotDTO);
            }
            return timeSlots;
        }

        [HttpPost("doctors/timeSlots/deleteTimeSlots")]
        public IActionResult DeleteTimeSlots(IEnumerable<DeleteTimeSlotsDTO> ids)
        {
            var result = FindAndDeleteDoctorTimeSlots(ids);
            return result;
        }

        private IActionResult FindAndDeleteDoctorTimeSlots(IEnumerable<DeleteTimeSlotsDTO> ids)
        {
            bool anyDeleted = false;
            foreach (DeleteTimeSlotsDTO deleteDTO in ids)
            {
                if (FindAndDeleteDoctorTimeSlot(deleteDTO.id))
                {
                    anyDeleted = true;
                }
            }
            _context.SaveChanges();
            if (anyDeleted)
                return Ok();
            return NotFound();


        }
        private bool FindAndDeleteDoctorTimeSlot(string timeSlotId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(timeSlotId);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            TimeSlot timeSlot;
            if ((timeSlot = _context.TimeSlots.Where(ts => ts.Active == true && ts.Id == id).SingleOrDefault()) != null)
            {
                Appointment appointment;
                if ((appointment = _context.Appointments.Where(a => a.State == AppointmentState.Planned && a.TimeSlotId == timeSlot.Id).SingleOrDefault()) != null)
                {
                    appointment.State = AppointmentState.Cancelled;
                    //poinformować pacjenta
                }
                timeSlot.Active = false;
                return true;
            }
            return false;
        }

        [HttpGet("vaccinationCenters")]
        public ActionResult<IEnumerable<VaccinationCenterDTO>> GetVaccinationCenters()
        {
            var result = GetAllVaccinationCenters();
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        private IEnumerable<VaccinationCenterDTO> GetAllVaccinationCenters()
        {
            List<VaccinationCenterDTO> result = new List<VaccinationCenterDTO>();
            foreach(VaccinationCenter center in _context.VaccinationCenters.ToList())
            {
                VaccinationCenterDTO centerDTO = new VaccinationCenterDTO();
                centerDTO.id = center.Id.ToString();
                centerDTO.name = center.Name;
                centerDTO.city = center.City;
                centerDTO.street = center.Address;
                List<VaccineInVaccinationCenterDTO> vaccines = new List<VaccineInVaccinationCenterDTO>();
                foreach (VaccinesInVaccinationCenter vaccine in _context.VaccinesInVaccinationCenter.Where(v => v.VaccinationCenterId == center.Id).Include("Vaccines").ToList())
                {
                    VaccineInVaccinationCenterDTO vaccineDTO = new VaccineInVaccinationCenterDTO();
                    var vaccineObject = _context.Vaccines.SingleOrDefault(v => v.Id == vaccine.VaccineId);
                    if(vaccineObject == null)
                    {
                        return null;
                    }
                    vaccine.Vaccine = vaccineObject;
                    vaccineDTO.id = vaccine.Vaccine.Id.ToString();
                    vaccineDTO.name = vaccine.Vaccine.Name;
                    vaccineDTO.companyName = vaccine.Vaccine.Company;
                    vaccineDTO.virus = vaccine.Vaccine.Virus.ToString();
                    vaccines.Add(vaccineDTO);
                }
                centerDTO.vaccines = vaccines;
                List<OpeningHoursDayDTO> openingHours = new List<OpeningHoursDayDTO>();

                List<OpeningHours> tempOpeningHours = _context.OpeningHours.Where(x => x.VaccinationCenterId == center.Id).ToList();
                tempOpeningHours.Sort(delegate(OpeningHours a, OpeningHours b)
                {
                    return a.WeekDay.CompareTo(b.WeekDay);
                });
                if (tempOpeningHours.Count != 7)
                {
                    Debug.WriteLine("Wrong opening hours");
                    return null;
                }
                int day = 0;
                foreach (OpeningHours oh in tempOpeningHours)
                {
                    if((int)oh.WeekDay != day)
                    {
                        Debug.WriteLine("Wrong opening hours day");
                        return null;
                    }
                    day++;
                    OpeningHoursDayDTO ohDTO = new OpeningHoursDayDTO();
                    try
                    {
                        ohDTO.from = oh.From.ToString(_timeSpanFormat);
                        ohDTO.to = oh.To.ToString(_timeSpanFormat);
                    }
                    catch(FormatException)
                    {
                        Debug.WriteLine("Wrong timespan format");
                        return null;
                    }
                    openingHours.Add(ohDTO);
                }
                centerDTO.openingHoursDays = openingHours;
                centerDTO.active = center.Active;
                result.Add(centerDTO);
                Debug.WriteLine("Added center");
            }
            return result;
        }

        [HttpPost("vaccinationCenters/addVaccinationCenter")]
        public IActionResult AddVaccinationCenter(AddVaccinationCenterRequestDTO addVaccinationCenterRequestDTO)
        {
            var result = AddNewVaccinationCenter(addVaccinationCenterRequestDTO);
            return result;
        }

        private IActionResult AddNewVaccinationCenter(AddVaccinationCenterRequestDTO addVaccinationCenterRequestDTO)
        {
            VaccinationCenter vaccinationCenter = new VaccinationCenter();
            vaccinationCenter.Id = Guid.NewGuid();
            vaccinationCenter.Name = addVaccinationCenterRequestDTO.name;
            vaccinationCenter.City = addVaccinationCenterRequestDTO.city;
            vaccinationCenter.Address = addVaccinationCenterRequestDTO.street;

            List<VaccinesInVaccinationCenter> vaccines = new List<VaccinesInVaccinationCenter>();
            List<OpeningHours> openingHours = new List<OpeningHours>();

            foreach(String vaccineId in addVaccinationCenterRequestDTO.vaccineIds)
            {
                Guid id;
                try
                {
                    id = Guid.Parse(vaccineId);
                }
                catch(FormatException)
                {
                    return NotFound();
                }
                catch(ArgumentNullException)
                {
                    return NotFound();
                }
                Vaccine vaccine;
                if((vaccine = _context.Vaccines.Where(vac => vac.Active == true && vac.Id == id).FirstOrDefault()) != null)
                {
                    VaccinesInVaccinationCenter vivc = new VaccinesInVaccinationCenter();
                    vivc.VaccinationCenterId = vaccinationCenter.Id;
                    vivc.VaccinationCenter = vaccinationCenter;
                    vivc.VaccineId = vaccine.Id;
                    vivc.Vaccine = vaccine;
                    vaccines.Add(vivc);
                    Debug.WriteLine("added vaccine");
                }
                else
                {
                    return NotFound();
                }
            }

            if (addVaccinationCenterRequestDTO.openingHoursDays.Count != 7)
                return NotFound();

            for(int i = 0;i < addVaccinationCenterRequestDTO.openingHoursDays.Count();i++)
            {
                OpeningHours oh = new OpeningHours();
                oh.VaccinationCenterId = vaccinationCenter.Id;
                oh.VaccinationCenter = vaccinationCenter;
                try
                {
                    oh.From = TimeSpan.ParseExact(addVaccinationCenterRequestDTO.openingHoursDays[i].from, _timeSpanFormat, null);
                    oh.To = TimeSpan.ParseExact(addVaccinationCenterRequestDTO.openingHoursDays[i].to, _timeSpanFormat, null);
                }
                catch(FormatException)
                {
                    return NotFound();
                }

                oh.WeekDay = (WeekDay)i;

                openingHours.Add(oh); 
            }

            vaccinationCenter.Active = addVaccinationCenterRequestDTO.active;
            foreach(var v in vaccines)
                _context.VaccinesInVaccinationCenter.Add(v);
            foreach(var oh in openingHours)
                _context.OpeningHours.Add(oh);
            _context.VaccinationCenters.Add(vaccinationCenter);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("vaccinationCenters/editVaccinationCenter")]
        public IActionResult EditVaccinationCenter(EditVaccinationCenterRequestDTO editVaccinationCenterRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("vaccinationCenters/deleteVaccinationCenter/{vaccinationCenterId}")]
        public IActionResult DeleteVaccinationCenter(string vaccinationCenterId)
        {
            var result = FindAndDeleteVaccinationCenter(vaccinationCenterId);
            return result;
        }

        private IActionResult FindAndDeleteVaccinationCenter(string vaccinationCenterId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(vaccinationCenterId);
            }
            catch(FormatException)
            {
                return NotFound();
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }
            VaccinationCenter vaccinationCenter;
            if((vaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Active == true && vc.Id == id).SingleOrDefault())!=null)
            {
                foreach(Doctor doctor in _context.Doctors.Where(doc => doc.Active == true && doc.VaccinationCenterId == id).ToList())
                {
                    foreach(var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).ToList())
                    {
                        appointment.State = AppointmentState.Cancelled;
                        var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
                        if(timeSlot == null)
                        {
                            return NotFound();
                        }
                        appointment.TimeSlot = timeSlot;
                        appointment.TimeSlot.Active = false;
                    }
                    _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == true).ToList().ForEach(ts => ts.Active = false);
                    doctor.Active = false;
                }
                vaccinationCenter.Active = false;
                _context.SaveChanges();
                    return Ok();
            }
            return NotFound();
        }

        [HttpGet("vaccines")]
        public ActionResult<IEnumerable<VaccineDTO>> GetVaccines() 
        {
            var result = GetAllVaccines();
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        private IEnumerable<VaccineDTO> GetAllVaccines()
        {
            List<VaccineDTO> vaccines = new List<VaccineDTO>();
            foreach(Vaccine vaccine in _context.Vaccines.ToList())
            {
                VaccineDTO vaccineDTO = new VaccineDTO();
                vaccineDTO.vaccineId = vaccine.Id.ToString();
                vaccineDTO.company = vaccine.Company;
                vaccineDTO.name = vaccine.Name;
                vaccineDTO.numberOfDoses = vaccine.NumberOfDoses;
                vaccineDTO.minDaysBetweenDoses = vaccine.MinDaysBetweenDoses;
                vaccineDTO.maxDaysBetweenDoses = vaccine.MaxDaysBetweenDoses;
                vaccineDTO.virus = vaccine.Virus.ToString();
                vaccineDTO.minPatientAge = vaccine.MinPatientAge;
                vaccineDTO.maxPatientAge = vaccine.MaxPatientAge;
                vaccineDTO.active = vaccine.Active;
                vaccines.Add(vaccineDTO);
            }
            return vaccines;
        }

        [HttpPost("vaccines/addVaccine")]
        public IActionResult AddVaccine(AddVaccineRequestDTO addVaccineRequestDTO)
        {
            var result = AddNewVaccine(addVaccineRequestDTO);
            return result;
        }

        private IActionResult AddNewVaccine(AddVaccineRequestDTO addVaccineRequestDTO)
        {
            Vaccine vaccine = new Vaccine();
            vaccine.Company = addVaccineRequestDTO.company;
            vaccine.Name = addVaccineRequestDTO.name;
            vaccine.NumberOfDoses = addVaccineRequestDTO.numberOfDoses;
            if (vaccine.NumberOfDoses < 1)
                return NotFound();
            vaccine.MinDaysBetweenDoses = addVaccineRequestDTO.minDaysBetweenDoses;
            vaccine.MaxDaysBetweenDoses = addVaccineRequestDTO.maxDaysBetweenDoses;
            if (vaccine.MinDaysBetweenDoses >= 0 && vaccine.MaxDaysBetweenDoses >= 0 && vaccine.MaxDaysBetweenDoses < vaccine.MinDaysBetweenDoses)
            {
                return NotFound();
            }
            try
            {
                vaccine.Virus = (Virus)Enum.Parse(typeof(Virus), addVaccineRequestDTO.virus);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (OverflowException)
            {
                return NotFound();
            }
            vaccine.MinPatientAge = addVaccineRequestDTO.minPatientAge;
            vaccine.MaxPatientAge = addVaccineRequestDTO.maxPatientAge;
            if (vaccine.MinPatientAge >= 0 && vaccine.MaxPatientAge >= 0 && vaccine.MaxPatientAge < vaccine.MinPatientAge)
            {
                return NotFound();
            }
            vaccine.Active = addVaccineRequestDTO.active;
            _context.Vaccines.Add(vaccine);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("vaccines/editVaccine")]
        public IActionResult EditVaccine(EditVaccineRequestDTO editVaccineRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("vaccines/deleteVaccine/{vaccineId}")]
        public IActionResult DeleteVaccine(string vaccineId)
        {
            var result = FindAndDeleteVaccine(vaccineId);
            return result;
        }

        private IActionResult FindAndDeleteVaccine(string vaccineId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(vaccineId);
            }
            catch(FormatException)
            {
                return NotFound();
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }
            Vaccine vaccine;
            if ((vaccine = _context.Vaccines.Where(vac => vac.Active == true && vac.Id == id).SingleOrDefault()) != null)
            {
                foreach(Appointment appointment in _context.Appointments.Where(a => a.State == AppointmentState.Planned && a.VaccineId == id).ToList())
                {
                    appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                }
                vaccine.Active = false;
                _context.SaveChanges();

                 return Ok();

            }
            return NotFound();
        }

    }
}
