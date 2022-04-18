using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class VaccinesInVaccinationCenter
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("VaccinationCenter")]
        public virtual Guid VaccinationCenterId { get; set; }
        public virtual VaccinationCenter VaccinationCenter { get; set; }
        [ForeignKey("Vaccine")]
        public virtual Guid VaccineId { get; set; }
        public virtual Vaccine Vaccine { get; set; }
    }
}
