using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class Certificate
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Url { get; set; }
        [ForeignKey("Patient")]
        public virtual Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        [ForeignKey("Vaccine")]
        public virtual Guid VaccineId { get; set; }
        public virtual Vaccine Vaccine { get; set; }

    }
}
