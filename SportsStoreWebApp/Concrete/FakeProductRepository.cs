using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Domain.Models;


namespace SportsStoreWebApp.Concrete
{
	public class FakeProductRepository : IProductRepository
	{
		private readonly List<Product> _products;

		public FakeProductRepository()
		{
			_products = new List<Product>
			{
				new Product { ProductID = 1, Name = "Bóng đá World Cup", Description = "Bóng đá chất lượng cao.", Price = 25.00m, Category = "Bóng đá", ImageUrl = "/images/football.jpg" },
				new Product { ProductID = 2, Name = "Bộ cờ vua chuyên nghiệp", Description = "Bộ cờ vua bằng gỗ cao cấp.", Price = 75.00m, Category = "Cờ vua", ImageUrl = "/images/chess.jpg" },
				new Product { ProductID = 3, Name = "Bóng chuyền bãi biển", Description = "Bóng chuyền dành cho bãi biển.", Price = 15.00m, Category = "Bóng chuyền", ImageUrl = "/images/volleyball.jpg" }
			};
		}

		public IQueryable<Product> Products => _products.AsQueryable();

		// Lưu (Create hoặc Update) - trả về Task để khớp interface async
		public Task SaveProduct(Product product)
		{
			if (product.ProductID == 0)
			{
				// tạo ID mới đơn giản (max + 1)
				var maxId = _products.Count == 0 ? 0 : _products.Max(p => p.ProductID);
				product.ProductID = maxId + 1;
				_products.Add(product);
			}
			else
			{
				var existing = _products.FirstOrDefault(p => p.ProductID == product.ProductID);
				if (existing != null)
				{
					existing.Name = product.Name;
					existing.Description = product.Description;
					existing.Price = product.Price;
					existing.Category = product.Category;
					existing.ImageUrl = product.ImageUrl;
				}
			}

			return Task.CompletedTask;
		}

		// Xóa (nếu interface yêu cầu Task<Product?>)
		public Task<Product?> DeleteProduct(int productId)
		{
			var item = _products.FirstOrDefault(p => p.ProductID == productId);
			if (item != null)
			{
				_products.Remove(item);
				return Task.FromResult<Product?>(item);
			}
			return Task.FromResult<Product?>(null);
		}
	}
}
