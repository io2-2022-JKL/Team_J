﻿using System.Collections.Generic;

namespace VaccinationSystem.DTO
{
    public class VaccinationCenterDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public List<VaccineInVaccinationCenterDTO> vaccines { get; set; }
        public List<OpeningHoursDayDTO> openingHoursDays { get; set; }
        public bool active { get; set; }
    }
}
