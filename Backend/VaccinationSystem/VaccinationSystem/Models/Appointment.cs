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
        public int Id { get; set; }
        [Required]
        public int WhichDose { get; set; }
        [ForeignKey("TimeSlot")]
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        [ForeignKey("Vaccine")]
        public int VaccineId { get; set; }
        public Vaccine Vaccine { get; set; }
        [Required]
        public AppointmentState State { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string VaccineBatchNumber { get; set; }
    
    }
}
