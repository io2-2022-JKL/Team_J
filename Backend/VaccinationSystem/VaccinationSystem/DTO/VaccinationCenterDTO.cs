using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaccinationSystem.DTO
{
    public class VaccinationCenterDTO
    {
        /// <example>837c1d09-8664-4480-beff-45fbd914c87e</example>
        [Required]
        public string id { get; set; }
        /// <example>Szpital przy Banacha</example>
        [Required]
        public string name { get; set; }
        /// <example>Warszawa</example>
        [Required]
        public string city { get; set; }
        /// <example>Stefana Banacha 1a</example>
        [Required]
        public string street { get; set; }
        /// <example>[{"id": "2dba46e3-a040-4dab-9ec4-600e44dbaf8e","name": "Pfizer","companyName": "BioNTech","virus": "Koronawirus"},{"id": "1fe2941e-f2f3-4edf-9ae3-712460e88ec7","name": "AstraZeneca","companyName": "Oxford","virus": "Koronawirus"}]</example>
        [Required]
        public List<VaccineInVaccinationCenterDTO> vaccines { get; set; }
        /// <example>[{"from": "07:00","to": "19:00"},{"from": "07:00","to": "19:00"},{"from": "07:00","to": "19:00"},{"from": "07:00","to": "19:00"},{"from": "07:00","to": "21:00"},{"from": "09:00","to": "16:00"},{"from": "00:00","to": "00:00"}]</example>
        [Required]
        public List<OpeningHoursDayDTO> openingHoursDays { get; set; }
        /// <example>true</example>
        [Required]
        public bool active { get; set; }
    }
}
