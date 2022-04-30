using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO.PatientDTOs
{
    public class PatientDTO
    {
        /// <example>acd9fa16-404e-4305-b57f-93659054be7e</example>
        [Required]
        public string id { get; set; }
        /// <example>68041377873</example>
        [Required]
        public string PESEL { get; set; }
        /// <example>James</example>
        [Required]
        public string firstName { get; set; }
        /// <example>Bond</example>
        [Required]
        public string lastName { get; set; }
        /// <example>james.bond@mi6.gov.uk</example>
        [Required]
        public string mail { get; set; }
        /// <example>13-04-1968</example>
        [Required]
        public string dateOfBirth { get; set; }
        /// <example>+44007</example>
        [Required]
        public string phoneNumber { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
