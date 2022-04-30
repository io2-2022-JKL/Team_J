using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class EditVaccinationCenterRequestDTO
    {
        /// <example>31ea1617-1bf9-48c1-8b40-0e2939bb6302</example>
        [Required]
        public string id { get; set; }
        /// <example>Szpital Uniwersytecki w Krakowie</example>
        [Required]
        public string name { get; set; }
        /// <example>Kraków</example>
        [Required]
        public string city { get; set; }
        /// <example>Macieja Jakubowskiego 2</example>
        [Required]
        public string street { get; set; }
        /// <example>["2dba46e3-a040-4dab-9ec4-600e44dbaf8e", "1fe2941e-f2f3-4edf-9ae3-712460e88ec7"]</example>
        [Required]
        public List<string> vaccineIds { get; set; }
        /// <example>[{"from": "07:00","to": "19:00"}, {"from": "07:00","to": "19:00"}, {"from": "07:00","to": "19:00"},{"from": "07:00","to": "19:00"},{"from": "07:00","to": "19:00"},{"from": "00:00","to": "00:00"},{"from": "00:00","to": "00:00"}]</example>
        [Required]
        public List<OpeningHoursDayDTO> openingHoursDays { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
