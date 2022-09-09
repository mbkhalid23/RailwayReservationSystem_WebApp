using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
    public class Train
    {
        [Key]
        public int TrainNo { get; set; }
        public string StationedAt { get; set; }
        public int Capacity { get; set; }
        public int SeatsAvailable { get; set; }
        public int SeatsBooked { get; set; }

        //Navigation Entries
        public ICollection<Schedule> Schedule { get; set; }
        public ICollection<Station> Station { get; set; }
    }
}
