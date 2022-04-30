using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO
{
    public class ExistingTimeSlotDTO
    {
        public string id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public bool isFree { get; set; }
    }
}
