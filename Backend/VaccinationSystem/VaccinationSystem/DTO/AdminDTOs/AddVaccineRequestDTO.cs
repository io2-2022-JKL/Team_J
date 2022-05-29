using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class AddVaccineRequestDTO
    {
        /// <example>WUM</example>
        [Required]
        public string company { get; set; }
        /// <example>WUMvaccine</example>
        [Required]
        public string name { get; set; }
        /// <example>1</example>
        [Required]
        public int numberOfDoses { get; set; }
        /// <example>-1</example>
        [Required]
        public int minDaysBetweenDoses { get; set; }
        /// <example>-1</example>
        [Required]
        public int maxDaysBetweenDoses { get; set; }
        /// <example>Koronawirus</example>
        [Required]
        public string virus { get; set; }
        /// <example>-1</example>
        [Required]
        public int minPatientAge { get; set; }
        /// <example>-1</example>
        [Required]
        public int maxPatientAge { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
