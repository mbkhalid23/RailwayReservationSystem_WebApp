using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
    public class Reservation
    {
        [Key]
        public int TicketNo { get; set; }

        //Navigation Entries
        public int PassengerId { get; set; }
        public Passenger Passenger { get; set; }
        public int? ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public int? StationId { get; set; }
        public Station Station { get; set; }
    }
}
