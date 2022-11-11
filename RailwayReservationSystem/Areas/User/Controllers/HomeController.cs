using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RailwayReservationSystem.DataAccess.Repository;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using System.Diagnostics;

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
                    s.Departure >= ScheduleView.Schedule.Departure).OrderBy(s => s.Departure).ToList();
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