using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO
{
    public class RegisterRequestDTO
    {
        /// <example>89042456235</example>
        [Required]
        public string PESEL { get; set; }
        /// <example>Jan</example>
        [Required]
        public string firstName { get; set; }
        /// <example>Kowalski</example>
        [Required]
        public string lastName { get; set; }
        /// <example>kowalskij@gmail.com</example>
        [Required]
        public string mail { get; set; }
        /// <example>24-04-1989</example>
        [Required]
        public string dateOfBirth { get; set; }
        /// <example>1234</example>
        [Required]
        public string password { get; set; }
        /// <example>+48 123 456 789</example>
        [Required]
        public string phoneNumber { get; set; }

    }
}
