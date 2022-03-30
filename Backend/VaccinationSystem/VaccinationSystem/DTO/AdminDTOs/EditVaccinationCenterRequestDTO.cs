using System.Collections.Generic;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class EditVaccinationCenterRequestDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public List<string> vaccineIds { get; set; }
        public List<OpeningHoursDayDTO> openingHoursDays { get; set; }
        public bool active { get; set; }
    }
}
