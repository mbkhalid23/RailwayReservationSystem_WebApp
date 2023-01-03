using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
    public class BookingCart
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        [ValidateNever]
        public Schedule Schedule { get; set; }
        public int Seats { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
