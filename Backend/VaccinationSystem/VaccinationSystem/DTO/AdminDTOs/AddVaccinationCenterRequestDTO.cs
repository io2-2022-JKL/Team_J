using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class AddVaccinationCenterRequestDTO
    {
        public string name { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public List<string> vaccineIds { get; set; }
        public List<OpeningHoursDayDTO> openingHoursDays { get; set; }
        public bool active { get; set; }
    }
}
