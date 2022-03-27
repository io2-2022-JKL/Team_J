using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class Vaccine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Company { get; set; }
        [Required] // Suggestion: Is it? Maybe allow nulls, and in case of nulls Name is equal to Company?
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Required]
        public int NumberOfDoses { get; set; }
        [Required]
        public int MinDaysBetweenDoses { get; set; }
        [Required]
        public int MaxDaysBetweenDoses { get; set; } // TODO: Need to add restriction that MaxDays >= MinDays
        [Required]
        public Virus Virus { get; set; }
        public int MinPatientAge { get; set; }
        public int MaxPatientAge { get; set; } // Suggestion: Vaccines could not have a minimum/maximum age, NULL if that's the case
        [Required]
        public bool Used { get; set; } // Suggestion: What is the point of it? Doesn't Vaccine represent a type of vaccine, not
                                       // a physical object?
    }
}