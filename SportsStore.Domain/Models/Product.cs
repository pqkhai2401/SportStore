namespace SportsStoreWebApp.Domain.Models
{
	public class Product
	{
		public int ProductID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string Category { get; set; } = string.Empty;
	}
}
