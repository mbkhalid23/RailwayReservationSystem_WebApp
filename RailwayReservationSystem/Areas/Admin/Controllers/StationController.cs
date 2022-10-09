using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
    public class StationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            //Get all stations
            IEnumerable<Station> stations = _unitOfWork.Station.GetAll();

            return View(stations);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Station obj)
        {

            if (ModelState.IsValid)
            {
                //Initialize trains stationed to 0 and available slots to max
                obj.TrainsStationed = 0;
                obj.AvailableSlots = obj.Capacity;

                //Add station to the database
                _unitOfWork.Station.Add(obj);
                _unitOfWork.Save();

                TempData["success"] = "Station added successfully";

                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Update(int? id)
        {
            //Check if id is null or 0
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Get station to pass it to the view
            Station obj = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == id);

            //Check if the station object is null
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Station obj)
        {
            //Check if the user accidently set the capacity station's capacity than the trains already stationed. We will lose data otherwise
            if (obj.Capacity < obj.TrainsStationed)
            {
                TempData["error"] = "Capcity can't be less than the trains already staioned";
                return View(obj);
            }
            
            if (ModelState.IsValid)
            {
                //Update the available slots on the station
                obj.AvailableSlots = obj.Capacity - obj.TrainsStationed;

                //Update station in the database
                _unitOfWork.Station.Update(obj);
                _unitOfWork.Save();

                TempData["success"] = "Station updated successfully";

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            //check if the id is null or 0
            if(id == null || id == 0)
            {
                return NotFound();
            }

            //Get station object to pass it to the view
            Station obj = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == id);

            //Check if the station object is null
            if(obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStation(int? id)
        {
            //Check if the is null or 0
            if(id == null || id == 0)
            {
                return NotFound();
            }

            //Get station
            Station obj = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == id);

            //Check if station object is null
            if (obj == null)
            {
                return NotFound();
            }

            //Check if there are any trains stationed at the given station
            if (obj.TrainsStationed > 0)
            {
                TempData["error"] = "Can't delete the station: There are some trains stationed here";
                return View(obj);
            }

            //Remove station from database
            _unitOfWork.Station.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Station deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
