using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VaccinationSystem.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(11)")]
        public string PESEL { get; set; } // Remember to place restriction on column so it only accepts digits
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Mail { get; set; }
        [Required]
        public string Password { get; set; } // Hashed of course
        [Required]
        [Column(TypeName = "varchar(15)")] // Not sure how many characters a phone number can have, with area codes like +48 or +633
                                           // Typed in 15 as a safe number, feel free to change it to something more accurate
        public string PhoneNumber { get; set; }
    }
}
