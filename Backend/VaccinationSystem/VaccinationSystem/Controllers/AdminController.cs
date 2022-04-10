using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.DTO.AdminDTOs;
using VaccinationSystem.Config;
using VaccinationSystem.Models;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;

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
            return StatusCode(404);
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
                    patientDTO.dateOfBirth = patient.DateOfBirth.ToString("dd-MM-yyyy");
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
                return StatusCode(404);
            }
            catch(ArgumentNullException)
            {
                return StatusCode(404);
            }
            Patient patient;
            if((patient = _context.Patients.Where(patient => patient.Active == true && patient.Id == id).FirstOrDefault()) != null)
            {
                Doctor doctor;
                if((doctor = _context.Doctors.Where(doc => doc.Active == true && doc.PatientAccount.Id == id).FirstOrDefault()) != null)
                {
                    doctor.Active = false;
                    foreach(Appointment appointment in doctor.Vaccinations)
                    {
                        if(appointment.State == AppointmentState.Planned)
                        {
                            appointment.State = AppointmentState.Cancelled;
                            appointment.TimeSlot.Active = false;
                            //powiadomić pacjentów
                        }
                    }
                    foreach(TimeSlot timeSlot in _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == false).ToList())
                    {
                        timeSlot.Active = false;
                    }
                }
                patient.Active = false;
                if(_context.SaveChanges() < 1)
                {
                    return StatusCode(404);
                }
                else
                {
                    return StatusCode(200);
                }
            }
            return StatusCode(404);
            
        }

        [HttpGet("doctors")]
        public ActionResult<IEnumerable<GetDoctorsResponseDTO>> GetDoctors()
        {
            var result = GetAllDoctors();
            if (result != null)
                return Ok(result);
            return StatusCode(404);
        }

        private IEnumerable<GetDoctorsResponseDTO> GetAllDoctors()
        {
            List<GetDoctorsResponseDTO> result = new List<GetDoctorsResponseDTO>();
            foreach(Doctor doc in _context.Doctors.ToList())
            {
                GetDoctorsResponseDTO getDoctorsResponseDTO = new GetDoctorsResponseDTO();
                getDoctorsResponseDTO.id = doc.Id.ToString();
                getDoctorsResponseDTO.PESEL = doc.PatientAccount.PESEL;
                getDoctorsResponseDTO.firstName = doc.PatientAccount.FirstName;
                getDoctorsResponseDTO.lastName = doc.PatientAccount.LastName;
                getDoctorsResponseDTO.mail = doc.PatientAccount.Mail;
                try
                {
                    getDoctorsResponseDTO.dateOfBirth = doc.PatientAccount.DateOfBirth.ToString("dd-MM-yyyy");
                }
                catch(FormatException)
                {
                    return null;
                }
                getDoctorsResponseDTO.phoneNumber = doc.PatientAccount.PhoneNumber;
                getDoctorsResponseDTO.active = doc.Active;
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
                id = Guid.Parse(addDoctorRequestDTO.doctorId);
            }
            catch (FormatException)
            {
                return StatusCode(404);
            }
            catch (ArgumentNullException)
            {
                return StatusCode(404);
            }
            Doctor doctor = new Doctor();
            doctor.PatientAccount = _context.Patients.Where(patient => patient.Active == true && patient.Id == id).FirstOrDefault();
            if(doctor.PatientAccount == null)
            {
                return StatusCode(404);
            }
            Guid vcId;
            try
            {
                vcId = Guid.Parse(addDoctorRequestDTO.vaccinationCenterId);
            }
            catch (FormatException)
            {
                return StatusCode(404);
            }
            catch (ArgumentNullException)
            {
                return StatusCode(404);
            }
            doctor.VaccinationCenterId = vcId;
            doctor.VaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Active == true && vc.Id == vcId).FirstOrDefault();
            if(doctor.VaccinationCenter == null)
            {
                return StatusCode(404);
            }
            doctor.Vaccinations = new List<Appointment>();
            var entry = _context.Doctors.Add(doctor);
            if (entry.State != Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                return StatusCode(404);
            }
            if (_context.SaveChanges() < 1)
            {
                return StatusCode(404);
            }
            return StatusCode(200);

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
                return StatusCode(404);
            }
            catch (ArgumentNullException)
            {
                return StatusCode(404);
            }

            Doctor doctor;
            if((doctor = _context.Doctors.Where(doc => doc.Active == true && doc.Id == id).SingleOrDefault()) != null)
            {             
                foreach (Appointment appointment in doctor.Vaccinations)
                {
                    if (appointment.State == AppointmentState.Planned)
                    {
                        appointment.State = AppointmentState.Cancelled;
                        appointment.TimeSlot.Active = false;
                        //powiadomić pacjentów
                    }
                }
                foreach (TimeSlot timeSlot in _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == false).ToList())
                {
                    timeSlot.Active = false;
                }
                doctor.Active = false;
                if (_context.SaveChanges() < 1)
                {
                    return StatusCode(404);
                }
                else
                {
                    return StatusCode(200);
                }
            }
            return StatusCode(404);

        }

        [HttpGet("vaccinationCenters")]
        public ActionResult<IEnumerable<VaccinationCenterDTO>> GetVaccinationCenters()
        {
            var result = GetAllVaccinationCenters();
            if (result != null)
                return Ok(result);
            return StatusCode(404);
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
                foreach(Vaccine vaccine in center.AvailableVaccines)
                {
                    VaccineInVaccinationCenterDTO vaccineDTO = new VaccineInVaccinationCenterDTO();
                    vaccineDTO.id = vaccine.Id.ToString();
                    vaccineDTO.name = vaccine.Name;
                    vaccineDTO.companyName = vaccine.Company;
                    vaccineDTO.virus = vaccine.Virus.ToString();
                    vaccines.Add(vaccineDTO);
                }
                centerDTO.vaccines = vaccines;
                List<OpeningHoursDayDTO> openingHours = new List<OpeningHoursDayDTO>();
                foreach(OpeningHours oh in center.OpeningHours)
                {
                    OpeningHoursDayDTO ohDTO = new OpeningHoursDayDTO();
                    try
                    {
                        ohDTO.From = oh.From.ToString(@"HH\:mm");
                        ohDTO.To = oh.To.ToString(@"HH\:mm");
                    }
                    catch(FormatException)
                    {
                        return null;
                    }
                    openingHours.Add(ohDTO);
                }
                centerDTO.openingHoursDays = openingHours;
                centerDTO.active = center.Active;
                result.Add(centerDTO);
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
            vaccinationCenter.Name = addVaccinationCenterRequestDTO.name;
            vaccinationCenter.City = addVaccinationCenterRequestDTO.city;
            vaccinationCenter.Address = addVaccinationCenterRequestDTO.street;
            List<Vaccine> vaccines = new List<Vaccine>();
            foreach(String vaccineId in addVaccinationCenterRequestDTO.vaccineIds)
            {
                Guid id;
                try
                {
                    id = Guid.Parse(vaccineId);
                }
                catch(FormatException)
                {
                    return StatusCode(404);
                }
                catch(ArgumentNullException)
                {
                    return StatusCode(404);
                }
                Vaccine vaccine;
                if((vaccine = _context.Vaccines.Where(vac => vac.Active == true && vac.Id == id).FirstOrDefault()) != null)
                {
                    vaccines.Add(vaccine);
                }
                else
                {
                    return StatusCode(404);
                }
            }
            vaccinationCenter.AvailableVaccines = vaccines;
            List<OpeningHours> openingHours = new List<OpeningHours>();
            foreach(OpeningHoursDayDTO ohDTO in addVaccinationCenterRequestDTO.openingHoursDays)
            {
                OpeningHours oh = new OpeningHours();
                try
                {
                    oh.From = TimeSpan.ParseExact(ohDTO.From, "HH:mm", null);
                    oh.To = TimeSpan.ParseExact(ohDTO.To, "HH:mm", null);
                }
                catch(FormatException)
                {
                    return StatusCode(404);
                }
                openingHours.Add(oh);
            }
            vaccinationCenter.OpeningHours = openingHours;
            vaccinationCenter.Doctors = new List<Doctor>();
            vaccinationCenter.Active = addVaccinationCenterRequestDTO.active;
            var entry = _context.VaccinationCenters.Add(vaccinationCenter);
            if (entry.State != Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                return StatusCode(404);
            }
            if (_context.SaveChanges() < 1)
            {
                return StatusCode(404);
            }
            return StatusCode(200);
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
                return StatusCode(404);
            }
            VaccinationCenter vaccinationCenter;
            if((vaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Active == true && vc.Id == id).SingleOrDefault())!=null)
            {
                foreach(Doctor doctor in vaccinationCenter.Doctors)
                {
                    if (doctor.Active)
                    {
                        foreach (Appointment appointment in doctor.Vaccinations)
                        {
                            if (appointment.State == AppointmentState.Planned)
                            {
                                appointment.State = AppointmentState.Cancelled;
                                appointment.TimeSlot.Active = false;
                                //powiadomić pacjentów
                            }
                        }
                        foreach (TimeSlot timeSlot in _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == false).ToList())
                        {
                            timeSlot.Active = false;
                        }
                        doctor.Active = false;
                    }
                }
                vaccinationCenter.Active = false;
                if (_context.SaveChanges() < 1)
                {
                    return StatusCode(404);
                }
                else
                {
                    return StatusCode(200);
                }
            }
            return StatusCode(404);
        }

        [HttpGet("vaccines")]
        public ActionResult<IEnumerable<VaccineDTO>> GetVaccines() 
        {
            var result = GetAllVaccines();
            if (result != null)
                return Ok(result);
            return StatusCode(404);
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
                return StatusCode(404);
            vaccine.MinDaysBetweenDoses = addVaccineRequestDTO.minDaysBetweenDoses;
            vaccine.MaxDaysBetweenDoses = addVaccineRequestDTO.maxDaysBetweenDoses;
            if (vaccine.MinDaysBetweenDoses >= 0 && vaccine.MaxDaysBetweenDoses >= 0 && vaccine.MaxDaysBetweenDoses < vaccine.MinDaysBetweenDoses)
            {
                return StatusCode(404);
            }
            try
            {
                vaccine.Virus = (Virus)Enum.Parse(typeof(Virus), addVaccineRequestDTO.virus);
            }
            catch (ArgumentNullException)
            {
                return StatusCode(404);
            }
            catch (ArgumentException)
            {
                return StatusCode(404);
            }
            catch (OverflowException)
            {
                return StatusCode(404);
            }
            vaccine.MinPatientAge = addVaccineRequestDTO.minPatientAge;
            vaccine.MaxPatientAge = addVaccineRequestDTO.maxPatientAge;
            if (vaccine.MinPatientAge >= 0 && vaccine.MaxPatientAge >= 0 && vaccine.MaxPatientAge < vaccine.MinPatientAge)
            {
                return StatusCode(404);
            }
            vaccine.Active = addVaccineRequestDTO.active;
            var entry = _context.Vaccines.Add(vaccine);
            if (entry.State != Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                return StatusCode(404);
            }
            if (_context.SaveChanges() < 1)
            {
                return StatusCode(404);
            }
            return StatusCode(200);
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
                return StatusCode(404);
            }
            Vaccine vaccine;
            if ((vaccine = _context.Vaccines.Where(vac => vac.Active == true && vac.Id == id).SingleOrDefault()) != null)
            {
                foreach(Appointment appointment in _context.Appointments.Where(a => a.State == AppointmentState.Planned && a.TimeSlot.Active == true && a.VaccineId == id).ToList())
                {
                    appointment.State = AppointmentState.Cancelled;
                }
                vaccine.Active = false;
                if (_context.SaveChanges() < 1)
                {
                    return StatusCode(404);
                }
                else
                {
                    return StatusCode(200);
                }
            }
            return StatusCode(404);
        }

        [HttpGet("doctors/timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<TimeSlotDTO>> GetTimeSlots(string doctorId)
        {
            var result = GetAllDoctorTimeSlots(doctorId);
            if (result != null)
                return Ok(result);
            return StatusCode(404);
        }

        private IEnumerable<TimeSlotDTO> GetAllDoctorTimeSlots(string doctorId)
        {
            List<TimeSlotDTO> timeSlots = new List<TimeSlotDTO>();
            Guid id;
            try
            {
                id = Guid.Parse(doctorId);
            }
            catch(FormatException)
            {
                return null;
            }
            foreach (TimeSlot timeSlot in _context.TimeSlots.Where(ts => ts.DoctorId == id).ToList())
            {
                TimeSlotDTO timeSlotDTO = new TimeSlotDTO();
                timeSlotDTO.id = timeSlot.Id.ToString();
                try
                {
                    timeSlotDTO.from = timeSlot.From.ToString(@"dd-MM-yyyy HH\:mm");
                    timeSlotDTO.to = timeSlot.To.ToString(@"dd-MM-yyyy HH\:mm");
                }
                catch(FormatException)
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
        public IActionResult DeleteTimeSlots(IEnumerable<string> ids)
        {
            var result = FindAndDeleteDoctorTimeSlots(ids);
            return result;
        }

        private IActionResult FindAndDeleteDoctorTimeSlots(IEnumerable<string> ids)
        {
            foreach(string stringId in ids)
            {
                if(!FindAndDeleteDoctorTimeSlot(stringId))
                {
                    return StatusCode(404);
                }
            }
            if (_context.SaveChanges() < 1)
            {
                return StatusCode(404);
            }
            else
            {
                return StatusCode(200);
            }

        }
        private bool FindAndDeleteDoctorTimeSlot(string timeSlotId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(timeSlotId);
            }
            catch(FormatException)
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

    }
}
