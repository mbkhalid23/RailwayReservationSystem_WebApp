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
        public int Capacity { get; set; }
        public int SeatsAvailable { get; set; }
        public int SeatsBooked { get; set; }

        //Navigation Entries
        public ICollection<Schedule>? Schedule { get; set; }

        [DisplayName("Stationed At")]
        public int? StationId { get; set; }
        public Station? Station { get; set; }
    }
}
