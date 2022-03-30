using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class DoctorMarkedAppointmentResponseDTO
    {
        public string vaccineName { get; set; }
        public string vaccineCompany { get; set; }
        public int numberOfDoses { get; set; }
        public int minDaysBetweenDoses { get; set; }
        public int maxDaysBetweenDoses { get; set; }
        public string virusName { get; set; }
        public int minPatientAge { get; set; }
        public int maxPatientAge { get; set; }
        public int whichVaccineDose { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public string PESEL { get; set; }
        public string dateOfBirth { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }
}
