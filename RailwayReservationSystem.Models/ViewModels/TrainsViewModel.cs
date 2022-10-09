using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models.ViewModels
{
    public class TrainsViewModel
    {
        public Train Train { get; set; }
        public IEnumerable<SelectListItem>? StationsList { get; set; }
        public int PrevStation { get; set; }
    }
}
