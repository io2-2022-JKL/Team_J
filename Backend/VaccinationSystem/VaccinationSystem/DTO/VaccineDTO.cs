using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO
{
    public class VaccineDTO
    {
        /// <example>2dba46e3-a040-4dab-9ec4-600e44dbaf8e</example>
        [Required]
        public string vaccineId { get; set; }
        /// <example>BioNTech</example>
        [Required]
        public string company { get; set; }
        /// <example>Pfizer</example>
        [Required]
        public string name { get; set; }
        /// <example>2</example>
        [Required]
        public int numberOfDoses { get; set; }
        /// <example>14</example>
        [Required]
        public int minDaysBetweenDoses { get; set; }
        /// <example>30</example>
        [Required]
        public int maxDaysBetweenDoses { get; set; }
        /// <example>Koronawirus</example>
        [Required]
        public string virus { get; set; }
        /// <example>18</example>
        [Required]
        public int minPatientAge { get; set; }
        /// <example>150</example>
        [Required]
        public int maxPatientAge { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
