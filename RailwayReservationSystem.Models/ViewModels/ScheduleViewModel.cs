﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models.ViewModels
{
    public class ScheduleViewModel
    {
        public Schedule Schedule { get; set; }
        public IEnumerable<Schedule> FullSchedule { get; set; }
        public Train Train { get; set; }
        public IEnumerable<SelectListItem>? TrainsList { get; set; }
    }
}