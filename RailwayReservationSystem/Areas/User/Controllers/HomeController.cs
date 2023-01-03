using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RailwayReservationSystem.DataAccess.Repository;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace RailwayReservationSystem.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        //GET
        public IActionResult Index()
        {
            ScheduleViewModel ScheduleView = new();
            {
                ScheduleView.Schedule = new();

                ScheduleView.StationsList = _unitOfWork.Station.GetAll().Select(
                    s => new SelectListItem
                    {
                        Text = s.City,
                        Value = s.StationId.ToString()
                    }).OrderBy(x => x.Text);

                ScheduleView.TrainsList = _unitOfWork.Train.GetAll().Select(
                    t => new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.TrainNo.ToString()
                    });
            };
            return View(ScheduleView);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ScheduleViewModel ScheduleView)
        {
            if (ScheduleView.Schedule.SourceStationId == 0 || ScheduleView.Schedule.DestinationStationId == 0)
            {
                TempData["error"] = "Please select a source and destination";
            }
            else if(ScheduleView.Schedule.Departure < DateTime.Today)
            {
                TempData["error"] = "Can't pick an older date";
            }
            else
            {
                ScheduleView.ScheduleList = _unitOfWork.Schedule.GetAll("Source,Destination,Train").Where(s =>
                    s.SourceStationId == ScheduleView.Schedule.SourceStationId &&
                    s.DestinationStationId == ScheduleView.Schedule.DestinationStationId &&
                    s.Departure.Date == ScheduleView.Schedule.Departure.Date).OrderBy(s => s.Departure);
            }

            ScheduleView.StationsList = _unitOfWork.Station.GetAll().Select(
                s => new SelectListItem
                {
                    Text = s.City,
                    Value = s.StationId.ToString()
                }).OrderBy(x => x.Text);

            ScheduleView.TrainsList = _unitOfWork.Train.GetAll().Select(
                t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.TrainNo.ToString()
                });

            return View(ScheduleView);
        }

        //GET
        public IActionResult Details(int scheduleId)
        {
            BookingCart cartObj = new();
            {
                cartObj.ScheduleId = scheduleId;
                cartObj.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(s => s.ScheduleId == scheduleId, IncludeProperties:"Source,Destination,Train");
                cartObj.Seats = 1;
            }

            return View(cartObj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(BookingCart bookingCart)
        {
            //Get the application user's identity
            //Extract the Name identifier since it is the id parameter for any user
            //Store the name identifier value as the application user identity
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            bookingCart.ApplicationUserId = claim.Value;

            //Get the existing cart from database in case user is adding the same element again
            //In this this case we would only need to update the count in the existing database entry
            BookingCart existingCart = _unitOfWork.BookingCart.GetFirstOrDefault(
                x => x.ApplicationUserId == claim.Value && x.ScheduleId == bookingCart.ScheduleId);

            //If the cart is empty, simply add the booking to the cart
            //else, increment the number of seats for the exisiting booking entry
            if (existingCart == null)
            {
                _unitOfWork.BookingCart.Add(bookingCart);
            }
            else
            {
                _unitOfWork.BookingCart.IncrementSeats(existingCart, bookingCart.Seats);
            }
            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}