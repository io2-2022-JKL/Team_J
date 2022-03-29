using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class OpeningHours
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TimeSpan From { get; set; }
        [Required]
        public TimeSpan To { get; set; }
    }
}
