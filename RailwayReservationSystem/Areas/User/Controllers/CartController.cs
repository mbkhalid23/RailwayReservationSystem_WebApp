using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using RailwayReservationSystem.Utility;
using Stripe.Checkout;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace RailwayReservationSystem.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;

		[BindProperty] //Bind this property so we don't have to pass it as a parameter in each POST method
        public BookingCartVM BookingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
			_emailSender = emailSender;
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
				item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId, IncludeProperties:"Source,Destination");

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

			//Stripe Settings
			var domain = "https://localhost:7139/";

			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>
				{
					"card",
				},
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"user/cart/OrderConfirmation?id={BookingCartVM.OrderHeader.Id}",
				CancelUrl = domain + $"user/cart/index",
			};

			foreach (var item in BookingCartVM.CartList)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Schedule.Fare * 100),
						Currency = "pkr",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Schedule.Source.City + " to " + item.Schedule.Destination.City,
						},
					},
					Quantity = item.Seats,
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);

			//Save sessionId and paymentIntentId so we can retrive it in order confirmation method
			_unitOfWork.OrderHeader.UpdateStripePaymentId(BookingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

		//GET the order confirmation page
		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, IncludeProperties:"ApplicationUser");

			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);

			//check the stripe status
			if (session.PaymentStatus.ToLower() == "paid")
			{
				//After new update stripe generates payment intent id after payment is processed, so we need to update it here
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.OrderStatusApproved, SD.PaymentStatusApproved);
				_unitOfWork.Save();
			}

			//Send confirmation Email
			_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Booking - Confirmed", "<p>Your Booking is confirmed</p>");

			//Now that the order is creatad, we will clear the booking cart
			//But first, we need to retrive the cart list

			List<BookingCart> bookingCarts = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			_unitOfWork.BookingCart.RemoveRange(bookingCarts);
			_unitOfWork.Save();

			return View(id);

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
                //Decrement session cart value by 1
                var count = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1; //subtracting -1 because we haven't saved the changes yet
                HttpContext.Session.SetInt32(SD.SessionCart, count); //Update session
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

            //Decrement session cart value by 1
            var count = _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, count); //Update session

            return RedirectToAction(nameof(Index));

		}
	}
}
