using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models.ViewModels
{
    public class BookingCartVM
    {
        public IEnumerable<BookingCart> CartList { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
