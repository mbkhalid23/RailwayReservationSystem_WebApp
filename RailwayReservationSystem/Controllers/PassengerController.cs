using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.Models;
using System.Linq;

namespace RailwayReservationSystem.Controllers
{
    public class PassengerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PassengerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var passengers = _db.Passengers.ToList();
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
            _db.Passengers.Add(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Passenger obj = _db.Passengers.Find(id);

            if(obj == null)
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
                _db.Passengers.Update(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Passenger obj = _db.Passengers.Find(id);

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
            if (id==null || id==0)
            {
                return NotFound();
            }

            Passenger obj = _db.Passengers.Find(id);

            if(obj==null)
            {
                return NotFound();
            }

            _db.Passengers.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
