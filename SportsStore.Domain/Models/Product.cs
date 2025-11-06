using System.ComponentModel.DataAnnotations;//Để dùng [key]

namespace SportsStoreWebApp.Domain.Models
{
	public class Product
	{
		// Class Product đại diện cho một sản phẩm trong cửa hàng
		[Key] // Đánh dấu đây là khóa chính
		public int ProductID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string Category { get; set; } = string.Empty;
	}
}
