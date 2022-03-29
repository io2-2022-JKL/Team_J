using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class Patient: User
    {
        public Dictionary<Virus, int> VaccinationsCount { get; set; }
        public IEnumerable<Appointment> VaccinationHistory { get; set; }
        public IEnumerable<Appointment> FutureVaccinations { get; set; }
        public IEnumerable<Certificate> Certificates { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
