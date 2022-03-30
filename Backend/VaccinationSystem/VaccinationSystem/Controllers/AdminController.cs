using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.DTO.AdminDTOs;

namespace VaccinationSystem.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("patients")]
        public ActionResult<IEnumerable<PatientDTO>> GetPatients()
        {
            return NotFound();
        }

        [HttpPost("patients/editPatient")]
        public IActionResult EditPatient(PatientDTO patientDTO)
        {
            return NotFound();
        }

        [HttpDelete("patients/deletePatient/{patientId}")]
        public IActionResult DeletePatient(string patientId)
        {
            return NotFound();
        }

        [HttpGet("doctors")]
        public ActionResult<IEnumerable<GetDoctorsResponseDTO>> GetDoctors()
        {
            return NotFound();
        }

        [HttpPost("doctors/editDoctor")]
        public IActionResult EditDoctor(EditDoctorRequestDTO editDoctorRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("doctors/addDoctor")]
        public IActionResult AddDoctor(AddDoctorRequestDTO addDoctorRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("doctors/deleteDoctor/{doctorId}")]
        public IActionResult DeleteDoctor(string doctorId)
        {
            return NotFound();
        }

        [HttpGet("vaccinationCenters")]
        public ActionResult<IEnumerable<VaccinationCenterDTO>> GetVaccinationCenters()
        {
            return NotFound();
        }

        [HttpPost("vaccinationCenters/addVaccinationCenter")]
        public IActionResult AddVaccinationCenter(AddVaccinationCenterRequestDTO addVaccinationCenterRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("vaccinationCenters/editVaccinationCenter")]
        public IActionResult EditVaccinationCenter(EditVaccinationCenterRequestDTO editVaccinationCenterRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("vaccinationCenters/deleteVaccinationCenter/{vaccinationCenterId}")]
        public IActionResult DeleteVaccinationCenter(string vaccinationCenterId)
        {
            return NotFound();
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
