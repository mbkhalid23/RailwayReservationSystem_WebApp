using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using System.Security.Claims;

namespace RailwayReservationSystem.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingCartVM BookingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {   
            //Get the user id from the ClaimsIdentity
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Get all the booking of the current user
            BookingCartVM = new()
            {
                CartList = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == claim.Value)
            };

			foreach (var item in BookingCartVM.CartList)
			{
				item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId, IncludeProperties: ("Source,Destination"));
                BookingCartVM.CartTotal += (item.Schedule.Fare * item.Seats);
			}

			return View(BookingCartVM);

        }

        //Increment number of seats of a cart item
		public IActionResult Plus(int cartId)
		{
            var cart = _unitOfWork.BookingCart.GetFirstOrDefault(x => x.Id == cartId);

            _unitOfWork.BookingCart.IncrementSeats(cart, 1);
            _unitOfWork.Save();

			return RedirectToAction(nameof(Index));

		}

        //Decrement number of seats of a cart item
		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.BookingCart.GetFirstOrDefault(x => x.Id == cartId);

            if (cart.Seats <= 1)
            {
				_unitOfWork.BookingCart.Remove(cart);
			}
            else
            {
				_unitOfWork.BookingCart.DecrementSeats(cart, 1);
			}
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));

		}

		public IActionResult Checkout(int cartId)
		{
			//var cart = _unitOfWork.BookingCart.GetFirstOrDefault(x => x.Id == cartId);

			//_unitOfWork.BookingCart.Remove(cart);
			//_unitOfWork.Save();

			return View();

		}

		//Remove a cart item
		public IActionResult Delete(int cartId)
		{
			var cart = _unitOfWork.BookingCart.GetFirstOrDefault(x => x.Id == cartId);

            _unitOfWork.BookingCart.Remove(cart);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));

		}
	}
}
