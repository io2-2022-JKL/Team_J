using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class GetCitiesDTO
    {
        /// <example>Warszawa</example>
        [Required]
        public string city { get; set; }
    }
}
