using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Utility
{
    public static class SD
    {
		//User Roles
        public const string Role_Passenger = "Passenger";
        public const string Role_Admin = "Admin";

		//Booking order status
		public const string OrderStatusPending = "Pending";
		public const string OrderStatusApproved = "Approved";
		public const string OrderStatusCancelled = "Cancelled";
		public const string OrderStatusRefunded = "Refunded";


		//Payment status
		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";
	}
}
