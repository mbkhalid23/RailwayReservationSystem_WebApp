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
            TrainsViewModel TrainsView = new();
            {
                TrainsView.Train = new();

                TrainsView.StationsList = _unitOfWork.Station.GetAll().Select(
                    s => new SelectListItem
                    {
                        Text = s.City + ", " + s.Name,
                        Value = s.StationId.ToString()
                    });
                
            };

                return View(TrainsView);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TrainsViewModel obj)
        {
            obj.Train.SeatsBooked = 0;
            obj.Train.SeatsAvailable = obj.Train.Capacity;

            if (ModelState.IsValid)
            {
                _unitOfWork.Train.Add(obj.Train);
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

            TrainsViewModel TrainsView = new();
            {
                TrainsView.Train = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id);

                if (TrainsView.Train == null)
                {
                    return NotFound();
                }

                TrainsView.StationsList = _unitOfWork.Station.GetAll().Select(
                    s => new SelectListItem
                    {
                        Text = s.City + ", " + s.Name,
                        Value = s.StationId.ToString()
                    });

            };

            return View(TrainsView);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(TrainsViewModel obj)
        {
            if (obj.Train.Capacity < obj.Train.SeatsBooked)
            {
                TempData["error"] = "Capacity cannot be less than seats booked";

                return View(obj);
            }

            obj.Train.SeatsAvailable = obj.Train.Capacity - obj.Train.SeatsBooked;

            if (ModelState.IsValid)
            {
                _unitOfWork.Train.Update(obj.Train);
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
