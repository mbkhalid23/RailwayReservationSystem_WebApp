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

            if (obj.TrainsStationed > obj.Capacity)
            {
                TempData["error"] = "Trains Stationed cannot be greater than Station's capacity";

                return View(obj);
            }

            obj.AvailableSlots = obj.Capacity - obj.TrainsStationed;

            if (ModelState.IsValid)
            {
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
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Station obj = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == id);

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
            if (obj.TrainsStationed > obj.Capacity)
            {
                TempData["error"] = "Trains Stationed cannot be greater than Station's capacity";

                return View(obj);
            }

            obj.AvailableSlots = obj.Capacity - obj.TrainsStationed;

            if (ModelState.IsValid)
            {
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
            if(id == null || id == 0)
            {
                return NotFound();
            }

            Station obj = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == id);

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
            if(id == null || id == 0)
            {
                return NotFound();
            }

            Station obj = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Station.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Station deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
