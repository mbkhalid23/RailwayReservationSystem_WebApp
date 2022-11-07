using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RailwayReservationSystem.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public DateTime Departure { get; set; } = DateTime.Now;

        public DateTime Arrival { get; set; } = DateTime.Now;
        
        public TimeSpan Journey { get; set; }

        //Navigation Entries
        [ForeignKey("Train")]
        public int TrainNo { get; set; }
        [ValidateNever]
        public Train Train { get; set; }

        [ForeignKey("Source")]
        public int SourceStationId { get; set; }
        [ValidateNever]
        public Station Source { get; set; }

        [ForeignKey("Destination")]
        public int DestinationStationId { get; set; }
        [ValidateNever]
        public Station Destination { get; set; }

        [ValidateNever]
        public ICollection<Reservation> Reservations { get; set; }
    }
}
