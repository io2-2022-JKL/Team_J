using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class DoctorFormerAppointmentDTO
    {
        public string VaccineName { get; set; }
        public string VaccineCompany { get; set; }
        public string VaccineVirus { get; set; }
        public int WhichVaccineDose { get; set; }
        public string AppointmentId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string PESEL { get; set; }
        public string State { get; set; }
        public string BatchNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
