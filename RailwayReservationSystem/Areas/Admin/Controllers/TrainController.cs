using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using System.Linq;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class TrainController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //Get all trains
            IEnumerable<Train> trains = _unitOfWork.Train.GetAll("Station");
            return View(trains);
        }

        //GET
        public IActionResult Create()
        {
            //Retrive a list of available stations for the dropdown
            TrainsViewModel TrainsView = new();
            {
                TrainsView.Train = new();

                TrainsView.StationsList = _unitOfWork.Station.GetAll().Select(
                    s => new SelectListItem
                    {
                        Text = s.City + ", " + s.Name,
                        Value = s.StationId.ToString()
                    }).OrderBy(x => x.Text);
                
            };

                return View(TrainsView);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TrainsViewModel TrainsView)
        {
            if (ModelState.IsValid)
            {

                if (TrainsView.Train.StationId == 0)
                {
                    TempData["error"] = "Please select a Station";
                    return View(TrainsView);
                }
                if (TrainsView.Train.Capacity < 0)
                {
                    TempData["error"] = "Capacity cannot be less than 0";

                    return View(TrainsView);
                }
                //Get station
                TrainsView.Train.Station = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == TrainsView.Train.StationId);

                //Check if the station has space available  for the train
                if (TrainsView.Train.Station.AvailableSlots <= 0)
                {
                    TempData["error"] = "Station already at Max capacity. Can't Add a new Train";
                    return View(TrainsView);
                }

                //Train arrives at the station
                TrainsView.Train.Station.TrainsStationed++;
                TrainsView.Train.Station.AvailableSlots--;

                //Add the train to database
                _unitOfWork.Train.Add(TrainsView.Train);
                _unitOfWork.Save();

                TempData["success"] = "Train added successfully";

                return RedirectToAction("Index");
            }

            return View(TrainsView);
        }

        //GET
        public IActionResult Update(int? id)
        {
            //Check if the id is null or 0
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Create a new ViewModel object to bind to the view
            TrainsViewModel TrainsView = new();
            {
                //Get train
                TrainsView.Train = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id);

                //Check if the train object is null
                if (TrainsView.Train == null)
                {
                    return NotFound();
                }

                //Retrive a list of available stations for the dropdown
                TrainsView.StationsList = _unitOfWork.Station.GetAll().Select(
                    s => new SelectListItem
                    {
                        Text = s.City + ", " + s.Name,
                        Value = s.StationId.ToString()
                    }).OrderBy(x=> x.Text);

                //Save the current station of train in the PrevStation variable
                TrainsView.PrevStation = TrainsView.Train.StationId;

            };

            return View(TrainsView);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(TrainsViewModel TrainsView)
        {
            if (ModelState.IsValid)
            {
                //Check if the train is moved to another station
                if(TrainsView.Train.StationId != TrainsView.PrevStation)
                {
                    //Get new station
                    TrainsView.Train.Station = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == TrainsView.Train.StationId);

                    //Check if new station has space available for a train
                    if(TrainsView.Train.Station.AvailableSlots <= 0)
                    {
                        TempData["error"] = "Station already at Max capacity. Can't move the Train";
                        return View(TrainsView);
                    }

                    //Train arrives at the new station
                    TrainsView.Train.Station.TrainsStationed++;
                    TrainsView.Train.Station.AvailableSlots--;

                    //Get prevoius station
                    Station PreviousStation = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == TrainsView.PrevStation);

                    //Train leaves the previous station
                    PreviousStation.TrainsStationed--;
                    PreviousStation.AvailableSlots++;

                    //Update the previous station in database
                    _unitOfWork.Station.Update(PreviousStation);
                }

                //Get a list of all the future schedules with the current Train
                List<Schedule> schedules = _unitOfWork.Schedule.GetAll(x => x.TrainNo == TrainsView.Train.TrainNo && x.Departure > DateTime.Now).ToList();

                //update avialable seats for each future schedule
                foreach (var schedule in schedules)
                {
                    schedule.SeatsAvailable = TrainsView.Train.Capacity - schedule.SeatsBooked;
                }

                //Update the train in the database
                _unitOfWork.Train.Update(TrainsView.Train);
                _unitOfWork.Save();

                TempData["success"] = "Train updated successfully";

                return RedirectToAction("Index");
            }

            return View(TrainsView);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            //Check if the id is null or 0
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Get train along with its station
            Train obj = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id, IncludeProperties:"Station");

            //Check if the train object is null
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
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

            //Get train along with its station
            Train obj = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id, IncludeProperties: "Station"); ;

            //Check if the train object is null
            if (obj == null)
            {
                return NotFound();
            }

            //Train leaves the station
            obj.Station.TrainsStationed--;
            obj.Station.AvailableSlots++;

            //Remove the train from database
            _unitOfWork.Train.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Train deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
