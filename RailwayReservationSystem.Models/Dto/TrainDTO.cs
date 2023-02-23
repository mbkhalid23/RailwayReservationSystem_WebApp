using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        //public enum TStatus
        //{
        //    [Display(Name = "In Service")] InService,
        //    [Display(Name = "In Maintenance")] InMaintenance,
        //    Retired
        //}

        [Required]
        public TrainStatus Status { get; set; }

        //Navigation Entries
        [ValidateNever]
        public ICollection<Schedule> Schedule { get; set; }

        [Required]
        [DisplayName("Stationed At")]
        public int StationId { get; set; }
        [ValidateNever]
        public Station Station { get; set; }
    }
}
