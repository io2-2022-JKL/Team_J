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
using System.ComponentModel.DataAnnotations;

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

        /// <remarks>Returns all patients</remarks>
        /// <response code="200">OK, found matching patients</response>
        /// <response code="401">Error, user unauthorized to search patients</response>
        /// <response code="403">Error, user forbidden from searching patients</response>
        /// <response code="404">Error, no patient found</response>
        [HttpGet("patients")]
        [ProducesResponseType(typeof(IEnumerable<PatientDTO>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

        /// <remarks>
        /// Edits patient's data
        /// </remarks>
        /// <param name="patientDTO"></param>
        /// <response code="200">Ok, edited patient</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to edit patient</response>
        /// <response code="403">Error, user forbidden from editing patient</response>
        /// <response code="404">Error, no patient found to edit</response>
        [HttpPost("patients/editPatient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult EditPatient(PatientDTO patientDTO)
        {
            return NotFound();
        }

        /// <remarks>Deletes a patient from system</remarks>
        /// <param name="patientId" example="f969ffd0-6dbc-4900-8eb8-b4fe25906a74"></param>
        /// <response code="200">Ok, deleted patient</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to delete patient</response>
        /// <response code="403">Error, user forbidden from deleting patient</response>
        /// <response code="404">Error, no patient found to delete</response>
        [HttpDelete("patients/deletePatient/{patientId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult DeletePatient([FromRoute]string patientId)
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
                return BadRequest();
            }
            catch(ArgumentNullException)
            {
                return BadRequest();
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
                            return BadRequest();
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

        /// <remarks>Returns all doctors</remarks>
        /// <response code="200">Ok, found matching doctors</response>
        /// <response code="401">Error, user unauthorized to search doctors</response>
        /// <response code="403">Error, user forbidden from searching doctors</response>
        /// <response code="404">Error, no doctor found</response>
        [HttpGet("doctors")]
        [ProducesResponseType(typeof(IEnumerable<GetDoctorsResponseDTO>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

        /// <remarks>Edits doctor's data</remarks>
        /// <param name="editDoctorRequestDTO"></param>
        /// <response code="200">Ok, edited doctor</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to edit doctor</response>
        /// <response code="403">Error, user forbidden from editing doctor</response>
        /// <response code="404">Error, no doctor found to edit</response>
        [HttpPost("doctors/editDoctor")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult EditDoctor([FromBody, Required] EditDoctorRequestDTO editDoctorRequestDTO)
        {
            return NotFound();
        }

        /// <remarks>Adds a new doctor</remarks>
        /// <param name="addDoctorRequestDTO"></param>
        /// <response code="200">Ok, added a new doctor</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to add doctors</response>
        /// <response code="403">Error, user forbidden from adding doctors</response>
        [HttpPost("doctors/addDoctor")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public IActionResult AddDoctor([FromBody, Required] AddDoctorRequestDTO addDoctorRequestDTO)
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
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            Doctor doctor = new Doctor();
            doctor.Id = Guid.NewGuid();
            doctor.PatientId = id;
            doctor.PatientAccount = _context.Patients.Where(patient => patient.Active == true && patient.Id == id).FirstOrDefault();
            if(doctor.PatientAccount == null)
            {
                return BadRequest();
            }
            if(_context.Doctors.Where(doc => doc.PatientId == id && doc.Active == true).Any())
            {
                return BadRequest();
            }
            Guid vcId;
            try
            {
                vcId = Guid.Parse(addDoctorRequestDTO.vaccinationCenterId);
            }
            catch (FormatException)
            {
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            doctor.VaccinationCenterId = vcId;
            doctor.VaccinationCenter = _context.VaccinationCenters.Where(vc => vc.Active == true && vc.Id == vcId).FirstOrDefault();
            if(doctor.VaccinationCenter == null)
            {
                return BadRequest();
            }

            doctor.Active = true;

            _context.Doctors.Add(doctor);
            Debug.WriteLine(_context.SaveChanges());
            return Ok();

        }

        /// <remarks>Deletes a doctor from system</remarks>
        /// <param name="doctorId" example="9d77b5e9-2823-4274-b326-d371e5582274"></param>
        /// <response code="200">Ok, deleted doctor</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to delete doctor</response>
        /// <response code="403">Error, user forbidden from deleting doctor</response>
        /// <response code="404">Error, no doctor found to delete</response>
        [HttpDelete("doctors/deleteDoctor/{doctorId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult DeleteDoctor([FromRoute]string doctorId)
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
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
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
                        return BadRequest();
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

        /// <remarks>Returns all time slots matching given criteria</remarks>
        /// <param name="doctorId" example="89a11879-4edf-4a67-a6f7-23c76763a418"></param>
        /// <response code="200">Ok, found mathing time slots</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to search time slots</response>
        /// <response code="403">Error, user forbidden from searching time slots</response>
        /// <response code="404">Error, no matching time slots or doctor found</response>
        [HttpGet("doctors/timeSlots/{doctorId}")]
        [ProducesResponseType(typeof(IEnumerable<TimeSlotDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<TimeSlotDTO>> GetTimeSlots([FromRoute]string doctorId)
        {
            var result = GetAllDoctorTimeSlots(doctorId);
            return result;
        }

        private ActionResult<IEnumerable<TimeSlotDTO>> GetAllDoctorTimeSlots(string doctorId)
        {
            List<TimeSlotDTO> timeSlots = new List<TimeSlotDTO>();
            Guid id;
            try
            {
                id = Guid.Parse(doctorId);
            }
            catch (FormatException)
            {
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            if (_context.Doctors.SingleOrDefault(doc => doc.Id == id) == null)
            {
                return NotFound();
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
                    return BadRequest();
                }
                timeSlotDTO.isFree = timeSlot.IsFree;
                timeSlotDTO.active = timeSlot.Active;
                timeSlots.Add(timeSlotDTO);
            }
            if (timeSlots.Count == 0)
                return NotFound();
            return Ok(timeSlots);
        }

        /// <remarks>Deletes time slots from system</remarks>
        /// <param name="ids"></param>
        /// <response code="200">Ok, deleted time slots</response>
        /// <response code="401">Error, user unauthorized to delete time slots</response>
        /// <response code="403">Error, user forbidden from deleting time slots</response>
        /// <response code="404">Error, no time slots found to delete</response>
        [HttpPost("doctors/timeSlots/deleteTimeSlots")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTimeSlots([FromBody, Required] IEnumerable<DeleteTimeSlotsDTO> ids)
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

        /// <remarks>Returns all vaccination centers matching given criteria</remarks>
        /// <response code="200">Ok, found vaccination centers</response>
        /// <response code="401">Error, user unauthorized to search vaccination centers</response>
        /// <response code="403">Error, user forbidden from searching vaccination centers</response>
        /// <response code="404">Error, no matching vaccination center found</response>
        [HttpGet("vaccinationCenters")]
        [ProducesResponseType(typeof(IEnumerable<VaccinationCenterDTO>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

        /// <remarks>Adds a vaccination center</remarks>
        /// <param name="addVaccinationCenterRequestDTO"></param>
        /// <response code="200">Ok, added vaccination center</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to add vaccination center</response>
        /// <response code="403">Error, user forbidden from adding vaccination center</response>
        /// <response code="404">Error, no vaccine found to add vaccination center</response>
        [HttpPost("vaccinationCenters/addVaccinationCenter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult AddVaccinationCenter([FromBody, Required] AddVaccinationCenterRequestDTO addVaccinationCenterRequestDTO)
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
                    return BadRequest();
                }
                catch(ArgumentNullException)
                {
                    return BadRequest();
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
                return BadRequest();

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
                    return BadRequest();
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

        /// <remarks>Edit vaccination center's data</remarks>
        /// <param name="editVaccinationCenterRequestDTO"></param>
        /// <response code="200">Ok, edited vaccination center</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to edit vaccination center</response>
        /// <response code="403">Error, user forbidden from editing vaccination center</response>
        /// <response code="404">Error, no vaccine found to edit vaccination center</response>
        [HttpPost("vaccinationCenters/editVaccinationCenter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult EditVaccinationCenter([FromBody, Required] EditVaccinationCenterRequestDTO editVaccinationCenterRequestDTO)
        {
            return NotFound();
        }

        /// <remarks>Deletes a vaccination center from system</remarks>
        /// <param name="vaccinationCenterId" example="250b86b0-28bf-4ca2-9322-0ff57953be8f"></param>
        /// <response code="200">Ok, deleted vaccination center</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to delete vaccination center</response>
        /// <response code="403">Error, user forbidden from deleting vaccination center</response>
        /// <response code="404">Error, no vaccination center found to delete</response>
        [HttpDelete("vaccinationCenters/deleteVaccinationCenter/{vaccinationCenterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult DeleteVaccinationCenter([FromRoute]string vaccinationCenterId)
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
                return BadRequest();
            }
            catch(ArgumentNullException)
            {
                return BadRequest();
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
                            return BadRequest();
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

        /// <remarks>Returns all vaccines</remarks>
        /// <response code="200">Ok, found vaccines</response>
        /// <response code="401">Error, user unauthorized to search vaccines</response>
        /// <response code="403">Error, user forbidden from searchig vaccines</response>
        /// <response code="404">Error, no matching vaccine found</response>
        [HttpGet("vaccines")]
        [ProducesResponseType(typeof(IEnumerable<VaccineDTO>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

        /// <remarks>Adds a vaccine</remarks>
        /// <param name="addVaccineRequestDTO"></param>
        /// <response code="200">Ok, added vaccine</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Error, user unauthorized to add vaccine</response>
        /// <response code="403">Error, user forbidden from editing vaccine</response>
        /// <response code="404">Error, virus not found</response>
        [HttpPost("vaccines/addVaccine")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult AddVaccine([FromBody, Required] AddVaccineRequestDTO addVaccineRequestDTO)
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
                return BadRequest();
            vaccine.MinDaysBetweenDoses = addVaccineRequestDTO.minDaysBetweenDoses;
            vaccine.MaxDaysBetweenDoses = addVaccineRequestDTO.maxDaysBetweenDoses;
            if (vaccine.MinDaysBetweenDoses >= 0 && vaccine.MaxDaysBetweenDoses >= 0 && vaccine.MaxDaysBetweenDoses < vaccine.MinDaysBetweenDoses)
            {
                return BadRequest();
            }
            try
            {
                vaccine.Virus = (Virus)Enum.Parse(typeof(Virus), addVaccineRequestDTO.virus);
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (OverflowException)
            {
                return BadRequest();
            }
            vaccine.MinPatientAge = addVaccineRequestDTO.minPatientAge;
            vaccine.MaxPatientAge = addVaccineRequestDTO.maxPatientAge;
            if (vaccine.MinPatientAge >= 0 && vaccine.MaxPatientAge >= 0 && vaccine.MaxPatientAge < vaccine.MinPatientAge)
            {
                return BadRequest();
            }
            vaccine.Active = addVaccineRequestDTO.active;
            _context.Vaccines.Add(vaccine);
            _context.SaveChanges();
            return Ok();
        }

        /// <remarks>Edits vaccine's data</remarks>
        /// <param name="editVaccineRequestDTO"></param>
        /// <response code="200">Ok, edited vaccine</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Error, user unauthorized to edit vaccines</response>
        /// <response code="403">Error, user forbidden from editing vaccines</response>
        /// <response code="404">Error, virus or vaccine not found</response>
        [HttpPost("vaccines/editVaccine")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult EditVaccine([FromBody, Required] EditVaccineRequestDTO editVaccineRequestDTO)
        {
            return NotFound();
        }

        /// <remarks>Deletes a vaccine from system</remarks>
        /// <param name="vaccineId" example="31d9b4bf-5c1c-4f2d-b997-f6096758eac9"></param>
        /// <response code="200">Ok, deleted vaccine</response>
        /// <response code="400">Bad data</response>
        /// <response code="401">Error, user unauthorized to delete vaccine</response>
        /// <response code="403">Error, user forbidden from deleting vaccine</response>
        /// <response code="404">Error, no vaccine found to delete</response>
        [HttpDelete("vaccines/deleteVaccine/{vaccineId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult DeleteVaccine([FromRoute]string vaccineId)
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
                return BadRequest();
            }
            catch(ArgumentNullException)
            {
                return BadRequest();
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
