using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using RailwayReservationSystem.Utility;
using System.Collections.Generic;
using System.Linq;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ScheduleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Schedule> ScheduleList = _unitOfWork.Schedule.GetAll(IncludeProperties:"Train,Source,Destination").OrderBy(x => x.Departure).ToList();

            return View(ScheduleList);
        }

        //GET
        public IActionResult Details(int? id)
        {
            ScheduleViewModel ScheduleView = new();
            {
                ScheduleView.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(s => s.ScheduleId == id, IncludeProperties: "Source,Destination,Train");
                ScheduleView.Seats = 1;
            }

            return View(ScheduleView);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ScheduleViewModel ScheduleView = new();
            {
                ScheduleView.Schedule = new();

                ScheduleView.StationsList = _unitOfWork.Station.GetAll().Select(
                    s => new SelectListItem
                    {
                        Text = s.City + ", " + s.Name,
                        Value = s.StationId.ToString()
                    }).OrderBy(x => x.Text);

                ScheduleView.TrainsList = _unitOfWork.Train.GetAll().Select(
                    t => new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.TrainNo.ToString()
                    });
            };

            if (id == null || id == 0)
            {
                //create a new schedule entry
                return View(ScheduleView);
            }

            else
            {
                //update existing schedule entry
                ScheduleView.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(s => s.ScheduleId == id);

                if (ScheduleView.Schedule == null)
                {
                    return NotFound();
                }

                return View(ScheduleView);

            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ScheduleViewModel ScheduleView)
        {
            if (ModelState.IsValid)
            {
                if(ScheduleView.Schedule.SourceStationId == 0 || ScheduleView.Schedule.DestinationStationId == 0)
                {
                    TempData["error"] = "Source and Destination can't be empty";
                    return View(ScheduleView);
                }

                if(ScheduleView.Schedule.SourceStationId == ScheduleView.Schedule.DestinationStationId)
                {
                    TempData["error"] = "Source and Destination can't be same";
                    return View(ScheduleView);
                }

                if(ScheduleView.Schedule.Arrival <= ScheduleView.Schedule.Departure)
                {
                    TempData["error"] = "Arrival can't be before or same as Departure";
                    return View(ScheduleView);
                }

                if (ScheduleView.Schedule.TrainNo != null)
                {
                    ScheduleView.Schedule.Train = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == ScheduleView.Schedule.TrainNo);

                    //Initialize seats booked and seats available
                    ScheduleView.Schedule.SeatsBooked = 0;
                    //Update the seats available corresponding to the updatedTrain
                    ScheduleView.Schedule.SeatsAvailable = ScheduleView.Schedule.Train.Capacity - ScheduleView.Schedule.SeatsBooked;
                }

                //Calculates the number of journey hours
                ScheduleView.Schedule.Journey = ScheduleView.Schedule.Arrival - ScheduleView.Schedule.Departure;

                //Add New Schedule entry to database
                if (ScheduleView.Schedule.ScheduleId == 0)
                {
                    _unitOfWork.Schedule.Add(ScheduleView.Schedule);
                    TempData["success"] = "Schedule entry added successfully";
                }

                //Update an existing Schedule entry to database
                else
                {
                    _unitOfWork.Schedule.Update(ScheduleView.Schedule);
                    TempData["success"] = "Schedule entry updated successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(ScheduleView);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            //Check if the id is null or 0
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Get schedule along with its source, destination, and train details
            Schedule schedule = _unitOfWork.Schedule.GetFirstOrDefault(s => s.ScheduleId == id, IncludeProperties: "Train,Source,Destination");

            //Check if the schedule object is null
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTrain(int? id)
        {
            //Check if the id is null or 0
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Get schedule along with its source, destination, and train details
            Schedule schedule = _unitOfWork.Schedule.GetFirstOrDefault(s => s.ScheduleId == id);

            //Check if the schedule object is null
            if (schedule == null)
            {
                return NotFound();
            }

            //Remove the schedule entry from database
            _unitOfWork.Schedule.Remove(schedule);
            _unitOfWork.Save();

            TempData["success"] = "Schedule entry deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
