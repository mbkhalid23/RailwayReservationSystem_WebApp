using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using RailwayReservationSystem.Utility;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace RailwayReservationSystem.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

		[BindProperty] //Bind this property so we don't have to pass it as a parameter in each POST method
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
				CartList = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == claim.Value),
				OrderHeader = new()
            };

			foreach (var item in BookingCartVM.CartList)
			{
				item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId, IncludeProperties: ("Source,Destination"));
                BookingCartVM.OrderHeader.OrderTotal += (item.Schedule.Fare * item.Seats);
			}

			return View(BookingCartVM);

        }

		public IActionResult Checkout()
		{
			//Get the user id from the ClaimsIdentity
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			//Get all the booking of the current user
			BookingCartVM = new()
			{
				CartList = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == claim.Value),
				OrderHeader = new()
			};

			//Populate the application user inside order header
			BookingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);

			BookingCartVM.OrderHeader.Name = BookingCartVM.OrderHeader.ApplicationUser.Name;
			BookingCartVM.OrderHeader.PhoneNumber = BookingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			BookingCartVM.OrderHeader.Age = BookingCartVM.OrderHeader.ApplicationUser.Age;
			BookingCartVM.OrderHeader.Gender = BookingCartVM.OrderHeader.ApplicationUser.Gender;

			//Include schedule property of each cart item
			//For each schedule, also include source and destination station
			//Also, calculate order total
			foreach (var item in BookingCartVM.CartList)
			{
				item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId, IncludeProperties: ("Source,Destination"));
				BookingCartVM.OrderHeader.OrderTotal += (item.Schedule.Fare * item.Seats);
			}

			return View(BookingCartVM);
		}

		//Checkout POST method
		[ActionName("Checkout")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CheckoutPOST()
		{
			//Get the user id from the ClaimsIdentity
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			//Get the Cart List from Database
			BookingCartVM.CartList = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == claim.Value);

			BookingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			BookingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
			BookingCartVM.OrderHeader.OrderDate = DateTime.Now;
			BookingCartVM.OrderHeader.ApplicationUserId = claim.Value;

			//Include schedule property of each cart item
			//For each schedule, also include source and destination station
			//Also, calculate order total
			foreach (var item in BookingCartVM.CartList)
			{
				item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId);

				//Check if there are any seats available
				if (item.Schedule.SeatsAvailable <= 0)
				{
					//No seats available
					TempData["error"] = "No Seats Available";

					return RedirectToAction("Index", "Home");
				}

				//Else, Update available and booked seats in the given schedule
				item.Schedule.SeatsAvailable -= item.Seats;
				item.Schedule.SeatsBooked += item.Seats;
				
				//Calculate order total for each item
				BookingCartVM.OrderHeader.OrderTotal += (item.Schedule.Fare * item.Seats);
			}

			//Add order header to unit of work and then save changes to database
			_unitOfWork.OrderHeader.Add(BookingCartVM.OrderHeader);
			_unitOfWork.Save();
			//Once the changes are saved Order header properties will be automatically loaded which we will use next

			//Now we need to create order details for all the items in the cart
			foreach (var item in BookingCartVM.CartList)
			{
				OrderDetail orderDetail = new()
				{
					ScheduleId = item.ScheduleId,
					OrderId = BookingCartVM.OrderHeader.Id,
					Fare = item.Schedule.Fare * item.Seats,
					Seats = item.Seats
				};
				//Add order detail to the unit of work and then save changed to database
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			//Now that the order is cretaed, we will clear the booking cart
			_unitOfWork.BookingCart.RemoveRange(BookingCartVM.CartList);
			_unitOfWork.Save();

			TempData["success"] = "Congratulations! Your booking order is placed";

			return RedirectToAction("Index", "Home");

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
