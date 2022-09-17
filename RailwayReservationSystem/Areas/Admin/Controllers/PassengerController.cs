using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using System.Linq;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
    public class PassengerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PassengerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Passenger> passengers = _unitOfWork.Passenger.GetAll();
            return View(passengers);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Passenger obj)
        {
            obj.Age = (DateTime.Now - obj.DOB).Days / 365;

            if (ModelState.IsValid)
            {
                _unitOfWork.Passenger.Add(obj);
                _unitOfWork.Save();

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

            Passenger obj = _unitOfWork.Passenger.GetFirstOrDefault(p => p.PassengerId == id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Passenger obj)
        {
            obj.Age = (DateTime.Now - obj.DOB).Days / 365;

            if (ModelState.IsValid)
            {
                _unitOfWork.Passenger.Update(obj);
                _unitOfWork.Save();

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

            Passenger obj = _unitOfWork.Passenger.GetFirstOrDefault(p => p.PassengerId == id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePassenger(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Passenger obj = _unitOfWork.Passenger.GetFirstOrDefault(p => p.PassengerId == id); ;

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Passenger.Remove(obj);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
