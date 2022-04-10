using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("VaccinationCenter")]
        public Guid VaccinationCenterId { get; set; }
        public VaccinationCenter VaccinationCenter { get; set; }
        //[Required]
        //[ForeignKey("ArchivedAppointment")]
        //public IEnumerable<Appointment> VaccinationsArchive { get; set; }
        //[Required]
        //[ForeignKey("FutureAppointment")]
        //public IEnumerable<Appointment> FutureVaccinations { get; set; }
        public IEnumerable<Appointment> Vaccinations { get; set; }
        [Required]
        public Patient PatientAccount { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
