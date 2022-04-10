using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class DoctorIncomingAppointmentDTO
    {
        public string VaccineName { get; set; }
        public string VaccineCompany { get; set; }
        public string VaccineVirus { get; set; }
        public int WhichVaccineDose { get; set; }
        public string AppointmentId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
