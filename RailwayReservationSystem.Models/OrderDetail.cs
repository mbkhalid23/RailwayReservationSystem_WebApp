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
	public class OrderDetail
	{
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }
		[ForeignKey("OrderId")]
		[ValidateNever]
		public OrderHeader OrderHeader { get; set; }

		[Required]
		public int ScheduleId { get; set; }
		[ForeignKey("ScheduleId")]
		[ValidateNever]
		public Schedule Schedule { get; set; }

		public int Seats { get; set; }
		public int Fare { get; set; }
	}
}
