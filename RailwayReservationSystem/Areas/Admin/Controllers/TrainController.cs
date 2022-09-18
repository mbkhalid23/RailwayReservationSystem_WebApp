using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using System.Linq;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
    public class TrainController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Train> trains = _unitOfWork.Train.GetAll();
            return View(trains);
        }

        //GET
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> StationList = _unitOfWork.Station.GetAll().Select(
                s => new SelectListItem
                {
                    Text = s.City + ", " + s.Name,
                    Value = s.StationId.ToString()
                });

            ViewBag.StationList = StationList;

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Train obj)
        {
            obj.SeatsBooked = 0;
            obj.SeatsAvailable = obj.Capacity;

            if (ModelState.IsValid)
            {
                _unitOfWork.Train.Add(obj);
                _unitOfWork.Save();

                TempData["success"] = "Train added successfully";

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

            Train obj = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id);

            if (obj == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> StationList = _unitOfWork.Station.GetAll().Select(
                s => new SelectListItem
                {
                    Text = s.City + ", " + s.Name,
                    Value = s.StationId.ToString()
                });

            ViewBag.StationList = StationList;

            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Train obj)
        {
            if (obj.Capacity < obj.SeatsBooked)
            {
                TempData["error"] = "Capacity cannot be less than seats booked";

                return View(obj);
            }

            obj.SeatsAvailable = obj.Capacity - obj.SeatsBooked;

            if (ModelState.IsValid)
            {
                _unitOfWork.Train.Update(obj);
                _unitOfWork.Save();

                TempData["success"] = "Train updated successfully";

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Train obj = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id);

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
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Train obj = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id); ;

            if (obj == null)
            {
                return NotFound();
            }

            if(obj.SeatsBooked != 0)
            {
                TempData["error"] = "Cannot delete train: it has some seats booked";

                return View(obj);
            }

            _unitOfWork.Train.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Train deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
