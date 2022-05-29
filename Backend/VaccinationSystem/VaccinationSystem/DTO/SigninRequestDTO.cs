using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO
{
    public class SigninRequestDTO
    {
        /// <example>james.bond@mi6.gov.uk</example>
        [Required]
        public string mail { get; set; }
        /// <example>AllH4ilTheQu33n</example>
        [Required]
        public string password { get; set; }
    }
}
