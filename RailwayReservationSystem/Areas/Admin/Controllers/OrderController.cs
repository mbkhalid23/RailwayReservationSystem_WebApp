using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Utility;
using System.Security.Claims;

namespace RailwayReservationSystem.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
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
	}
}
