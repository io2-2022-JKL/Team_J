using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class TimeSlotDTO
    {
        /// <example>5843d71b-574d-451e-be40-093fdb13983a</example>
        [Required]
        public string id { get; set; }
        /// <example>02-05-2022 13:00</example>
        [Required]
        public string from { get; set; }
        /// <example>02-05-2022 14:00</example>
        [Required]
        public string to { get; set; }
        /// <example>true</example>
        [Required]
        public bool isFree { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
