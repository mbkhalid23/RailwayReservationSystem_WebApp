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
    public class Station
    {
        [Key]
        public int StationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        public string City { get; set; }

        [Required]
        public int Capacity { get; set; }

        [DisplayName("Trains Stationed")]
        public int TrainsStationed { get; set; }

        [DisplayName("Available Slots")]
        public int AvailableSlots { get; set; }

        //Navigation entries
        public ICollection<Train>? Trains { get; set; }
    }
}
