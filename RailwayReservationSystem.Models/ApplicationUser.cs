using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RailwayReservationSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(15)]
        [StringLength(15, MinimumLength = 13, ErrorMessage = "CNIC must be 13 digits")]
        [RegularExpression(@"^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC must follow the XXXXX-XXXXXXX-X format!")]
        public string CNIC { get; set; }

        [MaxLength(6)]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public int? Age { get; set; }

        //Navigation Entries
        [ValidateNever]
        public ICollection<Reservation> Reservations { get; set; }
    }
}
