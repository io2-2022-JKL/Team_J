using System.Collections.Generic;

namespace VaccinationSystem.DTO.PatientDTOs
{
    public class TimeSlotFilterResponseDTO
    {
        public string timeSlotId { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
        public List<SimplifiedVaccineDTO> availableVaccines { get; set; }
        public List<OpeningHoursDayDTO> openingHours { get; set; }
        public string doctorFirstName { get; set; }
        public string doctorLastName { get; set; }
    }
}
