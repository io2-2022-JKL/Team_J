using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO
{
    public class SigninResponseDTO
    {
        /// <example>acd9fa16-404e-4305-b57f-93659054be7e</example>
        [Required]
        public string userId { get; set; }
        /// <example>patient</example>
        [Required]
        public string userType { get; set; }
    }
}
