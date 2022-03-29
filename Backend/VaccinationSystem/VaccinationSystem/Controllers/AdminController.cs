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
        [HttpPost("patients/showPatients")]
        public ActionResult<IEnumerable<PatientDTO>> GetPatients(GetPatientsRequestDTO getPatientsRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("patients/editPatient")]
        public IActionResult EditPatient(EditPatientRequestDTO editPatientRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("patients/deletePatient/{patientId}")]
        public IActionResult DeletePatient(string patientId)
        {
            return NotFound();
        }

        [HttpPost("doctors/showDoctors")]
        public ActionResult<IEnumerable<GetDoctorsResponseDTO>> GetDoctors(GetDoctorsRequestDTO getDoctorsRequestDTO)
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

        [HttpPost("vaccinationCenter/showVaccinationCenters")]
        public ActionResult<IEnumerable<VaccinationCenterDTO>> GetVaccinationCenters(GetVaccinationCentersRequestDTO getVaccinationCentersRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("doctors/editVaccinationCenter")]
        public IActionResult EditVaccinationCenter(EditVaccinationCenterRequestDTO editVaccinationCenterRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("doctors/deleteVaccinationCenter/{vaccinationCenterId}")]
        public IActionResult DeleteVaccinationCenter(string vaccinationCenterId)
        {
            return NotFound();
        }

        [HttpPost("patients/showVaccines")]
        public ActionResult<IEnumerable<VaccineDTO>> GetVaccines(GetVaccinesRequestDTO getVaccinesRequestDTO)
        {
            return NotFound();
        }

        [HttpPost("patients/editVaccine")]
        public IActionResult EditVaccine(EditVaccineRequestDTO editVaccineRequestDTO)
        {
            return NotFound();
        }

        [HttpDelete("patients/deleteVaccine/{vaccineId}")]
        public IActionResult DeleteVaccine(string vaccineId)
        {
            return NotFound();
        }
    }
}
