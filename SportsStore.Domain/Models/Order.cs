using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Models
{
	public class Order
	{
		public string CustomerName { get; set; } = string.Empty;
		public string ShippingAddress { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public string ProductName { get; set; } = string.Empty;
	}
}
