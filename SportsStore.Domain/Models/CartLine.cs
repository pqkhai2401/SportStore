using SportsStoreWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Models
{
	public class CartLine
	{
		public int CartLineID { get; set; }
		public Product Product { get; set; } = null!;
		public int Quantity { get; set; }
	}
}
