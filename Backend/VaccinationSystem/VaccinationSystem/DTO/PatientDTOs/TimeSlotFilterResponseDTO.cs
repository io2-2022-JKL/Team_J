using System.Collections.Generic;

namespace VaccinationSystem.DTO.PatientDTOs
{
    public class TimeSlotFilterResponseDTO
    {
        public string TimeSlotId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string VaccinationCenterName { get; set; }
        public string VaccinationCenterCity { get; set; }
        public string VaccinationCenterStreet { get; set; }
        public List<SimplifiedVaccineDTO> AvailableVaccines { get; set; }
        public List<OpeningHoursDayDTO> OpeningHours { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
    }
}
