using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
    public class Train
    {
        [Key]
        public int TrainNo { get; set; }

        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int Capacity { get; set; }

        [DisplayName("Seats Booked")]
        public int SeatsBooked { get; set; }

        [DisplayName("Seats Available")]
        public int SeatsAvailable { get; set; }

        //Navigation Entries
        [ValidateNever]
        public ICollection<Schedule> Schedule { get; set; }

        [Required]
        [DisplayName("Stationed At")]
        public int StationId { get; set; }
        [ValidateNever]
        public Station Station { get; set; }
    }
}
