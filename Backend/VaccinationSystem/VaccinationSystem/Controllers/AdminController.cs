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
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace VaccinationSystem.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly VaccinationSystemDbContext _context;
        private readonly string _dateFormat = "dd-MM-yyyy";
        private readonly string _dateTimeFormat = "dd-MM-yyyy HH\\:mm";
        private readonly string _timeSpanFormat = "hh\\:mm";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUri = "https://systemszczepienkonta.azurewebsites.net/";

        public AdminController(VaccinationSystemDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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
            var result = FindAndEditPatient(patientDTO).Result;
            return result;
        }

        private async Task<IActionResult> FindAndEditPatient(PatientDTO patientDTO)
        {
            if(patientDTO.id == null || !Guid.TryParse(patientDTO.id, out _))
            {
                return BadRequest();
            }

            Guid id = Guid.Parse(patientDTO.id);
            Patient patient = _context.Patients.SingleOrDefault(p => p.Id == id);

            if(patient != null)
            {
                DateTime dateOfBirth;
                string phoneNumber;

                if (patientDTO.PESEL == null || patientDTO.PESEL.Length != 11 || !ulong.TryParse(patientDTO.PESEL, out _))
                {
                    return BadRequest();
                }
                if(patientDTO.firstName == null || patientDTO.firstName.Length == 0 || DefaultController.ContainsSymbol(patientDTO.firstName))
                {
                    return BadRequest();
                }
                if(patientDTO.lastName == null || patientDTO.lastName.Length == 0 || DefaultController.ContainsSymbol(patientDTO.lastName))
                {
                    return BadRequest();
                }
                try
                {
                    dateOfBirth = DateTime.ParseExact(patientDTO.dateOfBirth, _dateFormat, null);
                }
                catch(FormatException)
                {
                    return BadRequest();
                }
                catch(ArgumentNullException)
                {
                    return BadRequest();
                }
                if(patientDTO.mail == null || !new EmailAddressAttribute().IsValid(patientDTO.mail))
                {
                    return BadRequest();
                }
                if(patientDTO.phoneNumber == null)
                {
                    return BadRequest();
                }
                phoneNumber = new string(patientDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                if(phoneNumber.Length == 0 || !DefaultController.IsPhoneNumber(phoneNumber))
                {
                    return BadRequest();
                }

                patient.FirstName = patientDTO.firstName;
                patient.LastName = patientDTO.lastName;
                patient.DateOfBirth = dateOfBirth;

                var httpClient = _httpClientFactory.CreateClient();

                if (patientDTO.active)
                {
                    if (_context.Patients.Any(p => p.Id != id && p.PESEL == patientDTO.PESEL && p.Active == true))
                    {
                        return BadRequest();
                    }
                    if( _context.Patients.Any(p => p.Id != id && p.Mail == patientDTO.mail && p.Active == true))
                    {
                        return BadRequest();
                    }

                    if (patient.Active)
                    {
                        if(patient.PhoneNumber != phoneNumber || patient.Mail != patientDTO.mail)
                        {
                            // zmiana danych 

                            string userId;
                            string role;
                            Doctor doctor = _context.Doctors.FirstOrDefault(d => d.PatientId == patient.Id && d.Active == true);
                            if (doctor == null)
                            {
                                userId = patientDTO.id;
                                role = Role.Patient;
                            }
                            else
                            {
                                userId = doctor.Id.ToString();
                                role = Role.Doctor;
                            }

                            var registerISDTO = new RegisterInIdentityServerDTO()
                            {
                                userId = userId,
                                email = patientDTO.mail,
                                phoneNumber = phoneNumber,
                                role = role,
                                password = patient.Password,
                            };
                            var registerJSON = JsonSerializer.Serialize(registerISDTO);

                            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "user/edit");
                            httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                            httpRequestMessage.Content = new StringContent(registerJSON, System.Text.Encoding.UTF8);
                            httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                            httpClient.BaseAddress = new Uri(_baseUri);

                            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                            if (!httpResponseMessage.IsSuccessStatusCode)
                                return BadRequest();

                        }
                    }
                    else
                    {
                        var registerISDTO = new RegisterInIdentityServerDTO()
                        {
                            userId = patient.Id.ToString(),
                            email = patientDTO.mail,
                            phoneNumber = phoneNumber,
                            role = Role.Patient,
                            password = patient.Password,
                        };
                        var registerJSON = JsonSerializer.Serialize(registerISDTO);

                        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "user/register");
                        httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        httpRequestMessage.Content = new StringContent(registerJSON, System.Text.Encoding.UTF8);
                        httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                        httpClient.BaseAddress = new Uri(_baseUri);

                        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                        if (!httpResponseMessage.IsSuccessStatusCode)
                            return BadRequest();
                    }
                }
                else
                {
                    if (patient.Active)
                    {
                        string userId;
                        Doctor doctor = _context.Doctors.FirstOrDefault(d => d.PatientId == patient.Id && d.Active == true);
                        if(doctor == null)
                        {
                            userId = patientDTO.id;
                        }
                        else
                        {
                            userId = doctor.Id.ToString();

                            foreach (var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).ToList())
                            {
                                appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                                var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
                                if (timeSlot == null)
                                {
                                    return BadRequest();
                                }
                                appointment.TimeSlot = timeSlot;
                                appointment.TimeSlot.Active = false;
                            }
                            _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == true).ToList().ForEach(slot => { slot.Active = false; });

                            doctor.Active = false;
                        }

                        var uri = Path.Combine("user/deletePatient", userId);
                        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        httpClient.BaseAddress = new Uri(_baseUri);

                        var response = await httpClient.SendAsync(request);
                        if (!response.IsSuccessStatusCode)
                            return BadRequest();
                    }
                }
                patient.PESEL = patientDTO.PESEL;
                patient.Mail = patientDTO.mail;
                patient.PhoneNumber = phoneNumber;
                patient.Active = patientDTO.active;

                _context.SaveChanges();

                return Ok();

            }

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
            var result = FindAndDeletePatient(patientId).Result;
            return result;
        }

        private async Task<IActionResult> FindAndDeletePatient(string patientId)
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
                var httpClient = _httpClientFactory.CreateClient();
                Doctor doctor = _context.Doctors.Where(doc => doc.Active == true && doc.PatientAccount.Id == id).FirstOrDefault();
                if(doctor != null)
                {
                    var uri = Path.Combine("user/deletePatient", doctor.Id.ToString());
                    var request = new HttpRequestMessage(HttpMethod.Delete, uri);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.BaseAddress = new Uri(_baseUri);

                    var response = await httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return BadRequest();

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
                else
                {
                    var uri = Path.Combine("user/deletePatient", patientId);
                    var request = new HttpRequestMessage(HttpMethod.Delete, uri);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.BaseAddress = new Uri(_baseUri);

                    var response = await httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return BadRequest();
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
            var result = FindAndEditDoctor(editDoctorRequestDTO).Result;
            return result;
        }

        private async Task<IActionResult> FindAndEditDoctor(EditDoctorRequestDTO doctorDTO)
        {
            if (doctorDTO.doctorId == null || !Guid.TryParse(doctorDTO.doctorId, out _))
            {
                return BadRequest();
            }

            if(doctorDTO.vaccinationCenterId == null || !Guid.TryParse(doctorDTO.vaccinationCenterId, out _))
            {
                return BadRequest();
            }

            Guid id = Guid.Parse(doctorDTO.doctorId);
            Doctor doctor = _context.Doctors.SingleOrDefault(d => d.Id == id);
            if(doctor == null)
            {
                return NotFound();
            }
            Patient patient = _context.Patients.SingleOrDefault(p => p.Id == doctor.PatientId);
            if(patient == null)
            {
                return BadRequest(); // nie powinno nigdy wystąpić
            }
            if(!_context.VaccinationCenters.Any(vc => vc.Id == doctor.VaccinationCenterId))
            {
                return BadRequest(); // nie powinno nigdy wystąpić
            }
            Guid vaccinationCenterId = Guid.Parse(doctorDTO.vaccinationCenterId);
            VaccinationCenter vaccinationCenter = _context.VaccinationCenters.SingleOrDefault(vc => vc.Id == vaccinationCenterId);

            if(vaccinationCenter == null)
            {
                return BadRequest();
            }

            DateTime dateOfBirth;
            string phoneNumber;

            if (doctorDTO.PESEL == null || doctorDTO.PESEL.Length != 11 || !ulong.TryParse(doctorDTO.PESEL, out _))
            {
                return BadRequest();
            }
            if (doctorDTO.firstName == null || doctorDTO.firstName.Length == 0 || DefaultController.ContainsSymbol(doctorDTO.firstName))
            {
                return BadRequest();
            }
            if (doctorDTO.lastName == null || doctorDTO.lastName.Length == 0 || DefaultController.ContainsSymbol(doctorDTO.lastName))
            {
                return BadRequest();
            }
            try
            {
                dateOfBirth = DateTime.ParseExact(doctorDTO.dateOfBirth, _dateFormat, null);
            }
            catch (FormatException)
            {
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            if (doctorDTO.mail == null || !new EmailAddressAttribute().IsValid(doctorDTO.mail))
            {
                return BadRequest();
            }
            if (doctorDTO.phoneNumber == null)
            {
                return BadRequest();
            }
            phoneNumber = new string(doctorDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            if (phoneNumber.Length == 0 || !DefaultController.IsPhoneNumber(phoneNumber))
            {
                return BadRequest();
            }

            patient.FirstName = doctorDTO.firstName;
            patient.LastName = doctorDTO.lastName;
            patient.DateOfBirth = dateOfBirth;

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_baseUri);

            if (doctorDTO.active)
            {
                if(!patient.Active)
                {
                    return BadRequest();
                }
                if (_context.Doctors.Any(d => d.Id != id && d.PatientId == patient.Id && d.Active == true))
                {
                    return BadRequest();
                }
                if (_context.Patients.Any(p => p.Id != patient.Id && p.PESEL == doctorDTO.PESEL && p.Active == true)) 
                {
                    return BadRequest();
                }
                if (_context.Patients.Any(p => p.Id != patient.Id && p.Mail == doctorDTO.mail && p.Active == true))
                {
                    return BadRequest();
                }
                if(!vaccinationCenter.Active)
                {
                    return BadRequest();
                }

                if (doctor.Active)
                {
                    if (patient.PhoneNumber != phoneNumber || patient.Mail != doctorDTO.mail)
                    {
                        // zmiana danych 

                        var registerISDTO = new RegisterInIdentityServerDTO()
                        {
                            userId = doctorDTO.doctorId,
                            email = doctorDTO.mail,
                            phoneNumber = phoneNumber,
                            role = Role.Doctor,
                            password = patient.Password,
                        };
                        var registerJSON = JsonSerializer.Serialize(registerISDTO);

                        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "user/edit");
                        httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        httpRequestMessage.Content = new StringContent(registerJSON, System.Text.Encoding.UTF8);
                        httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                        if (!httpResponseMessage.IsSuccessStatusCode)
                            return BadRequest();

                    }
                }
                else
                {
                    var uri = Path.Combine("user/addDoctor", doctor.PatientId.ToString(), doctor.Id.ToString());
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return BadRequest();

                    var registerISDTO = new RegisterInIdentityServerDTO()
                    {
                        userId = doctorDTO.doctorId,
                        email = doctorDTO.mail,
                        phoneNumber = phoneNumber,
                        role = Role.Doctor,
                        password = patient.Password,
                    };
                    var registerJSON = JsonSerializer.Serialize(registerISDTO);

                    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "user/edit");
                    httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    httpRequestMessage.Content = new StringContent(registerJSON, System.Text.Encoding.UTF8);
                    httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        return BadRequest();
                }
            }
            else
            {
                if (doctor.Active)
                {
                    var uri = Path.Combine("user/deleteDoctor", doctor.Id.ToString(), doctor.PatientId.ToString());
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return BadRequest();

                    var registerISDTO = new RegisterInIdentityServerDTO()
                    {
                        userId = doctor.PatientId.ToString(),
                        email = doctorDTO.mail,
                        phoneNumber = phoneNumber,
                        role = Role.Patient,
                        password = patient.Password,
                    };
                    var registerJSON = JsonSerializer.Serialize(registerISDTO);

                    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "user/edit");
                    httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    httpRequestMessage.Content = new StringContent(registerJSON, System.Text.Encoding.UTF8);
                    httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        return BadRequest();

                    foreach (var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).ToList())
                    {
                        appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                        var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
                        if (timeSlot == null)
                        {
                            return BadRequest();
                        }
                        appointment.TimeSlot = timeSlot;
                        appointment.TimeSlot.Active = false;
                    }
                    _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == true).ToList().ForEach(slot => { slot.Active = false; });

                }
            }
            doctor.Active = doctorDTO.active;
            patient.PESEL = doctorDTO.PESEL;
            patient.Mail = doctorDTO.mail;
            patient.PhoneNumber = phoneNumber;
            doctor.VaccinationCenterId = vaccinationCenterId;

            _context.SaveChanges();

            return Ok();
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
            var result = AddNewDoctor(addDoctorRequestDTO).Result;
            return result;
        }

        private async Task<IActionResult> AddNewDoctor(AddDoctorRequestDTO addDoctorRequestDTO)
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

            var uri = Path.Combine("user/addDoctor", doctor.PatientId.ToString(), doctor.Id.ToString());
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_baseUri);

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest();

            _context.Doctors.Add(doctor);
            _context.SaveChanges();
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
            var result = FindAndDeleteDoctor(doctorId).Result;
            return result;
        }

        private async Task<IActionResult> FindAndDeleteDoctor(string doctorId)
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
                var uri = Path.Combine("user/deleteDoctor", doctorId, doctor.PatientId.ToString());
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_baseUri);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return BadRequest();

                foreach (var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).ToList())
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
                    if(vaccineObject.Active)
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
            if(addVaccinationCenterRequestDTO == null || addVaccinationCenterRequestDTO.name == null || addVaccinationCenterRequestDTO.city == null || addVaccinationCenterRequestDTO.street == null)
            {
                return BadRequest();
            }
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
                if((vaccine = _context.Vaccines.Where(vac => vac.Id == id).FirstOrDefault()) != null)
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
            var result = FindAndEditVaccinationCenter(editVaccinationCenterRequestDTO).Result;
            return result;
        }

        private async Task<IActionResult> FindAndEditVaccinationCenter(EditVaccinationCenterRequestDTO vaccinationCenterDTO)
        {
            if(vaccinationCenterDTO == null)
            {
                return BadRequest();
            }
            if(vaccinationCenterDTO.id == null || !Guid.TryParse(vaccinationCenterDTO.id, out _))
            {
                return BadRequest();
            }
            if(vaccinationCenterDTO.vaccineIds == null)
            {
                return BadRequest();
            }
            if(vaccinationCenterDTO.openingHoursDays == null || vaccinationCenterDTO.openingHoursDays.Count() != 7)
            {
                return BadRequest();
            }
            if(vaccinationCenterDTO.city == null || vaccinationCenterDTO.street == null || vaccinationCenterDTO.name == null)
            {
                return BadRequest();
            }

            var id = Guid.Parse(vaccinationCenterDTO.id);

            VaccinationCenter vaccinationCenter = _context.VaccinationCenters.FirstOrDefault(vc => vc.Id == id);

            if(vaccinationCenter != null)
            {
                for (int i = 0; i < vaccinationCenterDTO.openingHoursDays.Count(); i++)
                {
                    OpeningHours oh = _context.OpeningHours.FirstOrDefault(oh => oh.VaccinationCenterId == id && oh.WeekDay == (WeekDay)i);
                    if(oh == null)
                    {
                        return BadRequest();
                    }
                    try
                    {
                        oh.From = TimeSpan.ParseExact(vaccinationCenterDTO.openingHoursDays[i].from, _timeSpanFormat, null);
                        oh.To = TimeSpan.ParseExact(vaccinationCenterDTO.openingHoursDays[i].to, _timeSpanFormat, null);
                    }
                    catch (FormatException)
                    {
                        return BadRequest();
                    }
                }
                foreach (var vivc in _context.VaccinesInVaccinationCenter.Where(v => v.VaccinationCenterId == id).ToList())
                    _context.VaccinesInVaccinationCenter.Remove(vivc);

                foreach(string vaccineIdString in vaccinationCenterDTO.vaccineIds)
                {
                    Guid vaccineId;
                    try
                    {
                        vaccineId = Guid.Parse(vaccineIdString);
                    }
                    catch (FormatException)
                    {
                        return BadRequest();
                    }
                    catch (ArgumentNullException)
                    {
                        return BadRequest();
                    }
                    Vaccine vaccine;
                    if ((vaccine = _context.Vaccines.Where(vac => vac.Id == vaccineId).FirstOrDefault()) != null)
                    {
                        VaccinesInVaccinationCenter vivc = new VaccinesInVaccinationCenter();
                        vivc.VaccinationCenterId = vaccinationCenter.Id;
                        vivc.VaccinationCenter = vaccinationCenter;
                        vivc.VaccineId = vaccine.Id;
                        vivc.Vaccine = vaccine;
                        _context.VaccinesInVaccinationCenter.Add(vivc);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                vaccinationCenter.Name = vaccinationCenterDTO.name;
                vaccinationCenter.City = vaccinationCenterDTO.city;
                vaccinationCenter.Address = vaccinationCenterDTO.street;

                if(!vaccinationCenterDTO.active && vaccinationCenter.Active)
                {
                    // zamiana danych lekarzy

                    foreach(var doc in _context.Doctors.Where(d => d.VaccinationCenterId == id && d.Active == true).ToList())
                    {
                        var uri = Path.Combine("user/deleteDoctor", doc.Id.ToString(), doc.PatientId.ToString());
                        var request = new HttpRequestMessage(HttpMethod.Get, uri);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var httpClient = _httpClientFactory.CreateClient();
                        httpClient.BaseAddress = new Uri(_baseUri);

                        var response = await httpClient.SendAsync(request);
                        if (!response.IsSuccessStatusCode)
                            return BadRequest();

                        /*foreach (var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doc.Id && a.State == AppointmentState.Planned).ToList())
                        {
                            appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                            var timeSlot = _context.TimeSlots.SingleOrDefault(ts => ts.Id == appointment.TimeSlotId);
                            if (timeSlot == null)
                            {
                                return BadRequest();
                            }
                            appointment.TimeSlot = timeSlot;
                            appointment.TimeSlot.Active = false;
                        }
                        _context.TimeSlots.Where(slot => slot.DoctorId == doc.Id && slot.Active == true).ToList().ForEach(slot => { slot.Active = false; });
                        */

                        foreach (var timeSlot in _context.TimeSlots.Where(ts => ts.DoctorId == doc.Id && ts.Active == true).ToList())
                        {
                            var appointment = _context.Appointments.SingleOrDefault(a => a.TimeSlotId == timeSlot.Id && a.State == AppointmentState.Planned);
                            if (appointment != null)
                            {
                                appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                            }
                            timeSlot.Active = false;
                        }

                        doc.Active = false;
                    }
                }

                vaccinationCenter.Active = vaccinationCenterDTO.active;

                _context.SaveChanges();
                return Ok();

            }

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
            var result = FindAndDeleteVaccinationCenter(vaccinationCenterId).Result;
            return result;
        }

        private async Task<IActionResult> FindAndDeleteVaccinationCenter(string vaccinationCenterId)
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
                var httpClient = _httpClientFactory.CreateClient();
                foreach (Doctor doctor in _context.Doctors.Where(doc => doc.Active == true && doc.VaccinationCenterId == id).ToList())
                {
                    var uri = Path.Combine("user/deleteDoctor", doctor.Id.ToString(), doctor.PatientId.ToString());
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.BaseAddress = new Uri(_baseUri);

                    var response = await httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return BadRequest();

                    /*foreach (var appointment in _context.Appointments.Include("TimeSlots").Where(a => a.TimeSlot.DoctorId == doctor.Id && a.State == AppointmentState.Planned).ToList())
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
                    _context.TimeSlots.Where(slot => slot.DoctorId == doctor.Id && slot.Active == true).ToList().ForEach(ts => ts.Active = false);*/

                    foreach (var timeSlot in _context.TimeSlots.Where(ts => ts.DoctorId == doctor.Id && ts.Active == true).ToList())
                    {
                        var appointment = _context.Appointments.SingleOrDefault(a => a.TimeSlotId == timeSlot.Id && a.State == AppointmentState.Planned);
                        if (appointment != null)
                        {
                            appointment.State = AppointmentState.Cancelled; // powiadomić pacjentów
                        }
                        timeSlot.Active = false;
                    }

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
            var result = FindAndEditVaccine(editVaccineRequestDTO);
            return result;
        }

        private IActionResult FindAndEditVaccine(EditVaccineRequestDTO vaccineDTO)
        {
            if (vaccineDTO == null || vaccineDTO.vaccineId == null || vaccineDTO.company == null || vaccineDTO.name == null || vaccineDTO.virus == null)
            {
                return BadRequest();
            }

            Guid id;
            try
            {
                id = Guid.Parse(vaccineDTO.vaccineId);
            }
            catch (FormatException)
            {
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }

            var vaccine = _context.Vaccines.FirstOrDefault(v => v.Id == id);

            if(vaccine == null)
            {
                return NotFound();
            }

            vaccine.Company = vaccineDTO.company;
            vaccine.Name = vaccineDTO.name;

            try
            {
                vaccine.Virus = (Virus)Enum.Parse(typeof(Virus), vaccineDTO.virus);
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

            if(vaccineDTO.numberOfDoses < 1)
            {
                return BadRequest();
            }

            vaccine.NumberOfDoses = vaccineDTO.numberOfDoses;

            if (vaccineDTO.minDaysBetweenDoses >= 0 && vaccineDTO.maxDaysBetweenDoses >= 0 && vaccineDTO.maxDaysBetweenDoses < vaccineDTO.minDaysBetweenDoses)
            {
                return BadRequest();
            }

            vaccine.MinDaysBetweenDoses = vaccineDTO.minDaysBetweenDoses;
            vaccine.MaxDaysBetweenDoses = vaccineDTO.maxDaysBetweenDoses;

            if (vaccineDTO.minPatientAge >= 0 && vaccineDTO.maxPatientAge >= 0 && vaccineDTO.maxPatientAge < vaccineDTO.minPatientAge)
            {
                return BadRequest();
            }

            vaccine.MaxPatientAge = vaccineDTO.maxPatientAge;
            vaccine.MinPatientAge = vaccineDTO.minPatientAge;

            vaccine.Active = vaccineDTO.active;

            _context.SaveChanges();
            return Ok();
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
