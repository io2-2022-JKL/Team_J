using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class FutureAppointmentDTO
    {
        public string vaccineName { get; set; }
        public string vaccineCompany { get; set; }
        public string vaccineVirus { get; set; }
        public int whichVaccineDose { get; set; }
        public string appointmentId { get; set; }
        public string windowBegin { get; set; }
        public string windowEnd { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
        public string doctorFirstName { get; set; }
        public string doctorLastName { get; set; }
    }
}
