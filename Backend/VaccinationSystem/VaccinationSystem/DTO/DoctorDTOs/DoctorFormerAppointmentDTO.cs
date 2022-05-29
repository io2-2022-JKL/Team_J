using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class DoctorFormerAppointmentDTO
    {
        public string vaccineName { get; set; }
        public string vaccineCompany { get; set; }
        public string vaccineVirus { get; set; }
        public int whichVaccineDose { get; set; }
        public string appointmentId { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public string PESEL { get; set; }
        public string state { get; set; }
        public string batchNumber { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string certifyState { get; set; }
    }
}
