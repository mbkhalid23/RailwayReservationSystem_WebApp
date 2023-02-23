using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RailwayReservationSystem.Models.Train;

namespace RailwayReservationSystem.Models.Dto
{
    public class TrainCreateDTO
    {
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public TrainStatus Status { get; set; }
        [Required]
        public int StationId { get; set; }
    }
}
