using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Utility;
using System.Security.Claims;

namespace RailwayReservationSystem.ViewComponents
{
    public class BookingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Get the user from database
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.BookingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
