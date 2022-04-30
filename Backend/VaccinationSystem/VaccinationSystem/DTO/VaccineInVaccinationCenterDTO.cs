using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class VaccineInVaccinationCenterDTO
    {
        /// <example>2dba46e3-a040-4dab-9ec4-600e44dbaf8e</example>
        [Required]
        public string id { get; set; }
        /// <example>Pfizer</example>
        [Required]
        public string name { get; set; }
        /// <example>BioNTech</example>
        [Required]
        public string companyName { get; set; }
        /// <example>Koronawirus</example>
        [Required]
        public string virus { get; set; }
    }
}
