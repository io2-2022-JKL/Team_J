using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class ExistingTimeSlotDTO
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool IsFree { get; set; }
    }
}
