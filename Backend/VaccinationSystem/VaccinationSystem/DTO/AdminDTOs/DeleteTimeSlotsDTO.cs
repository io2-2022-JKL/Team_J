using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.DTO.AdminDTOs
{
    public class DeleteTimeSlotsDTO
    {
        /// <example>02f2b999-d0ba-4f33-9a9a-69e1cae2952b</example>
        [Required]
        public string id { get; set; }
    }
}
