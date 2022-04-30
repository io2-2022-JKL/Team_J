using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int WhichDose { get; set; }
        [ForeignKey("TimeSlot")]
        public virtual Guid? TimeSlotId { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }
        [ForeignKey("Patient")]
        public virtual Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        [ForeignKey("Vaccine")]
        public virtual Guid VaccineId { get; set; }
        public virtual Vaccine Vaccine { get; set; }
        [Required]
        public AppointmentState State { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string VaccineBatchNumber { get; set; }
        public CertifyState CertifyState { get; set; }
    
    }
}
