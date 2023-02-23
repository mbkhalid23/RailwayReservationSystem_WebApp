using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static RailwayReservationSystem.Models.Train;

namespace RailwayReservationSystem.Models.Dto
{
    public class TrainDTO
    {
        [Key]
        public int TrainNo { get; set; }
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
