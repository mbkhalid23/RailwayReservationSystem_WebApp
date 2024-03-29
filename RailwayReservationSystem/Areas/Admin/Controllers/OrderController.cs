﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.ViewModels;
using RailwayReservationSystem.Utility;
using Stripe;
using Stripe.Checkout;
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

		//POST method to allow admin to Pay for the Order
		[ActionName("Details")]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Details_PAY_NOW()
        {
			OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id, IncludeProperties: "ApplicationUser");
			OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == OrderVM.OrderHeader.Id, IncludeProperties: "Schedule");

            foreach (var item in OrderVM.OrderDetail)
            {
                item.Schedule = _unitOfWork.Schedule.GetFirstOrDefault(x => x.ScheduleId == item.ScheduleId, IncludeProperties: "Source,Destination");
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
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
            };

            foreach (var item in OrderVM.OrderDetail)
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
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        //GET the payment confirmation page
        public IActionResult PaymentConfirmation(int orderHeaderid)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderHeaderid);

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            //check the stripe status
            if (session.PaymentStatus.ToLower() == "paid")
            {
                //After new update stripe generates payment intent id after payment is processed, so we need to update it here
                _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderid, orderHeader.SessionId, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderid, SD.OrderStatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            return View(orderHeaderid);

        }

        //POST method to update order details
        [HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult UpdateOrderDetails()
        {
			//Retrive order header from db based on the order header id recieved
			var OrderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id, tracked: false);

			//update order header details
			OrderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			OrderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;

			//update the changes and save to database
			_unitOfWork.OrderHeader.Update(OrderHeaderFromDb);
			_unitOfWork.Save();

			TempData["success"] = "Order Details Updated Successfully";

            return RedirectToAction("Details", "Order", new { orderId = OrderHeaderFromDb.Id});
        }

        //POST method to cancel an order 
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelOrder()
        {
            //Retrive order header from db based on the order header id recieved
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id, tracked: false);

			//If payment is already made, we must refund it
			if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,
					//here you can also mention specific amount to refund
					//however, if left blank, it will refund the original order total
				};

				//Now we need to create a refund service
				//We then create a refund object and call the service create function and pass options to it
				//refund process will start
				var service = new RefundService();
				Refund refund = service.Create(options);

				//Now we need to update order status
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderStatusCancelled, SD.PaymentStatusRefunded);

			}

			//if payment is not made, we just update the order status
			else
			{
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderStatusCancelled, SD.OrderStatusCancelled);
            }

            //Now we need to update available seats in schedule
            //To do that we will first get Order details from db and include Schedule
            var orderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == orderHeader.Id, IncludeProperties: "Schedule");
            //Next we will update the available seats and seats booked in the schedule entries retrived
            foreach (var item in orderDetails)
            {
                item.Schedule.SeatsAvailable += item.Seats;
                item.Schedule.SeatsBooked -= item.Seats;
            }

            //Now save changes to database
            _unitOfWork.Save();

            TempData["success"] = "Order Canceller Successfully";

            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
        }
    }
}
