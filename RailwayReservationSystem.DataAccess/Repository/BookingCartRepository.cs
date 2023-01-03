using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Repository
{
    public class BookingCartRepository : Repository<BookingCart>, IBookingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrementSeats(BookingCart bookingCart, int seats)
        {
            bookingCart.Seats -= seats;
            return bookingCart.Seats;
        }

        public int IncrementSeats(BookingCart bookingCart, int seats)
        {
            bookingCart.Seats += seats;
            return bookingCart.Seats;
        }
    }
}
