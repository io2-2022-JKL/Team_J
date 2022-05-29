using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class AddDoctorRequestDTO
    {
        /// <example>f982aaa8-4be7-4115-a4f9-6cbab37ae726</example>
        [Required]
        public string patientId { get; set; }
        /// <example>837c1d09-8664-4480-beff-45fbd914c87e</example>
        [Required]
        public string vaccinationCenterId { get; set; }
    }
}
