using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(15)]
        [StringLength(15, MinimumLength = 13, ErrorMessage ="CNIC must be 13 digits")]
        [RegularExpression(@"^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC must follow the XXXXX-XXXXXXX-X format!")]
        public string CNIC { get; set; }

        [MaxLength(6)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        public int Age { get; set; }

        [Required]
        [MaxLength(100)]
        //[RegularExpression(@"^[\w -\.] +@([\w -] +\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        [DisplayName("Phone No.")]
        [MaxLength(12)]
        [StringLength(12, MinimumLength = 11, ErrorMessage ="Phone No must be 11 digits")]
        [RegularExpression(@"^03[0-9]{2}-[0-9]{7}",ErrorMessage ="Phone No. must follow the 03XX-XXXXXXX format!")]
        public string PhoneNo { get; set; }

        //Navigation Entries
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
