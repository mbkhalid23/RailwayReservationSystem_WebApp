using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Repository.IRepository
{
    public interface IBookingCartRepository : IRepository<BookingCart>
    {
        int IncrementSeats(BookingCart bookingCart, int seats);
        int DecrementSeats(BookingCart bookingCart, int seats);
    }
}
