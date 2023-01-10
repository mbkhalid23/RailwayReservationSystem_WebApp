using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using RailwayReservationSystem.Utility;
using System.Security.Claims;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		//GET method displays a list of all the orders for admin and individual order for users
		public IActionResult Index()
		{
			List<OrderHeader> orderHeaders;

			//If the user is Admin then Get all orders. Else, since the user will be customer, get only order of that specific customer
			if (User.IsInRole(SD.Role_Admin))
			{
                orderHeaders = _unitOfWork.OrderHeader.GetAll(IncludeProperties: "ApplicationUser").ToList();
            }
			else
			{
				//Get the application user from claims identity
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

				//Get the orders for current user
				orderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claim.Value, IncludeProperties: "ApplicationUser").ToList();
			}

            return View(orderHeaders);
		}

		//GET method to display Order Details
        public IActionResult Details(int orderId)
        {
			OrderVM = new OrderVM()
			{
				OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId, IncludeProperties: "ApplicationUser"),

				OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == orderId),
			};

			foreach (var item in OrderVM.OrderDetail)
			{
                item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId, IncludeProperties: "Source,Destination");
            }

            return View(OrderVM);
        }


    }
}
