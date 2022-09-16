using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Source { get; set; }

        [Required]
        [MaxLength(30)]
        public string Destination { get; set; }

        public DateTime Departure { get; set; }
        
        public DateTime Arrival { get; set; }
        
        public TimeSpan Journey { get; set; }

        //Navigation Entries
        [ForeignKey("Train")]
        public int? TrainNo { get; set; }
        public Train Train { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
