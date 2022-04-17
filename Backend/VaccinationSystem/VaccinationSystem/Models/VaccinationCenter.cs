using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class VaccinationCenter
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(40)")]
        public string City { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Address { get; set; }
        //[Required]
        //public IEnumerable<OpeningHours> OpeningHours { get; set; }
        //[Required]
        //public IEnumerable<Doctor> Doctors { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
