using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }

		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		[Required]
		public DateTime OrderDate { get; set; }
		public int OrderTotal { get; set; }
		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public DateTime PaymentDate { get; set; }

		//For stripe payments
		public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }

		//User Details
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int Age { get; set; }
		[Required]
		public string Gender { get; set; }

	}
}
