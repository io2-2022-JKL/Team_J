using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO.PatientDTOs
{
    public class PatientInfoResponseDTO
    {
        /// <example>Maryla</example>
        [Required]
        public string firstName { get; set; }
        /// <example>Rodowicz</example>
        [Required]
        public string lastName { get; set; }
        /// <example>03055458741</example>
        [Required]
        public string PESEL { get; set; }
        /// <example>13-04-1968</example>
        [Required]
        public string dateOfBirth { get; set; }
        /// <example>james.bond@mi6.gov.uk</example>
        [Required]
        public string mail { get; set; }
        /// <example>+48123456789</example>
        [Required]
        public string phoneNumber { get; set; }
    }
}
