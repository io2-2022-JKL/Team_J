using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class GetVirusDTO
    {
        /// <example>Koronawirus</example>
        /// <example>Grypa</example>
        [Required]
        public string virus { get; set; }
    }
}
