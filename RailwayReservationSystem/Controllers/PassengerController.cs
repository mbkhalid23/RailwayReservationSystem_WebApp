using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Data;


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
    }
}
