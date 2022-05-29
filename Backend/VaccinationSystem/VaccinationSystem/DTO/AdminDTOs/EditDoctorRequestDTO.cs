using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class EditDoctorRequestDTO
    {
        /// <example>89a11879-4edf-4a67-a6f7-23c76763a418</example>
        [Required]
        public string doctorId { get; set; }
        /// <example>70013131688</example>
        [Required]
        public string PESEL { get; set; }
        /// <example>Aaeesh'a</example>
        [Required]
        public string firstName { get; set; }
        /// <example>Abd Al-Rashid</example>
        [Required]
        public string lastName { get; set; }
        /// <example>aaeeshaAAR@doktor.org.pl</example>
        [Required]
        public string mail { get; set; }
        /// <example>31-01-1970</example>
        [Required]
        public string dateOfBirth { get; set; }
        /// <example>+48 923 219 001</example>
        [Required]
        public string phoneNumber { get; set; }
        /// <example>837c1d09-8664-4480-beff-45fbd914c87e</example>
        [Required]
        public string vaccinationCenterId { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
