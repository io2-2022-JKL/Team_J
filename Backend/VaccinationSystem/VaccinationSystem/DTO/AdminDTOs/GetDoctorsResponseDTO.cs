using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class GetDoctorsResponseDTO
    {
        /// <example>89a11879-4edf-4a67-a6f7-23c76763a418</example>
        [Required]
        public string id { get; set; }
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
        /// <example>863928017</example>
        [Required]
        public string phoneNumber { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
        /// <example>837c1d09-8664-4480-beff-45fbd914c87e</example>
        [Required]
        public string vaccinationCenterId { get; set; }
        /// <example>Szpital przy Banacha</example>
        [Required]
        public string name { get; set; }
        /// <example>Warszawa</example>
        [Required]
        public string city { get; set; }
        /// <example>Stefana Banacha 1a</example>
        [Required]
        public string street { get; set; }
    }
}
