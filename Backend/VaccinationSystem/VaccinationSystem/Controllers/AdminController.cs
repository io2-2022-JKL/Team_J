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
                        ohDTO.From = oh.From.ToString(@"hh\:mm");
                        ohDTO.To = oh.To.ToString(@"hh\:mm");
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
                    oh.From = TimeSpan.ParseExact(ohDTO.From, "hh:mm", null);
                    oh.To = TimeSpan.ParseExact(ohDTO.To, "hh:mm", null);
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
        public ActionResult<IEnumerable<VaccineDTO>> GetVaccines(VaccineDTO vaccineDTO) 
        { 
            return NotFound();
        }

        [HttpPost("vaccines/addVaccine")]
        public IActionResult AddVaccine(AddVaccineRequestDTO addVaccineRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("vaccines/editVaccine")]
        public IActionResult EditVaccine(EditVaccineRequestDTO editVaccineRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("vaccines/deleteVaccine/{vaccineId}")]
        public IActionResult DeleteVaccine(string vaccineId)
        {
            return NotFound();
        }

        [HttpGet("doctors/timeSlots/{doctorId}")]
        public ActionResult<IEnumerable<TimeSlotDTO>> GetTimeSlots()
        {
            return NotFound();
        }

        [HttpPost("doctors/timeSlots/deleteTimeSlots")]
        public IActionResult DeleteTimeSlots(IEnumerable<string> ids)
        {
            return NotFound();
        }
    }
}
