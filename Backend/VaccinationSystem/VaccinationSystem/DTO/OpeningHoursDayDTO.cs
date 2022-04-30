using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class OpeningHoursDayDTO
    {
        /// <example>07:00</example>
        [Required]
        public string from { get; set; }
        /// <example>19:00</example>
        [Required]
        public string to { get; set; }
    }
}
