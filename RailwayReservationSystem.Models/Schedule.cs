using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace RailwayReservationSystem.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public DateTime Departure { get; set; } = DateTime.Today;

        [Required]
        public DateTime Arrival { get; set; } = DateTime.Today;
        
        public TimeSpan Journey { get; set; }

        [DisplayName("Seats Booked")]
        public int SeatsBooked { get; set; }

        [DisplayName("Seats Available")]
        public int SeatsAvailable { get; set; }

        //Navigation Entries
        [ForeignKey("Train")]
        public int? TrainNo { get; set; }
        [ValidateNever]
        public Train Train { get; set; }

        [Required]
        [ForeignKey("Source")]
        public int SourceStationId { get; set; }
        [ValidateNever]
        public Station Source { get; set; }

        [Required]
        [ForeignKey("Destination")]
        public int DestinationStationId { get; set; }
        [ValidateNever]
        public Station Destination { get; set; }

        [ValidateNever]
        public ICollection<Reservation> Reservations { get; set; }
    }
}
