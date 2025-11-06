using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;
using SportsStoreWebApp.Domain.Models;
using System.Collections.Generic;

namespace SportsStoreWebApp.Controllers
{
	// Áp dụng một route tiền tố cho toàn bộ Controller nếu muốn dùngAttribute Routing mạnh mẽ
	// [Route("san-pham")] // Ví dụ: mọi action sẽ bắt đầu bằng /cua-hang/
	public class ProductController : Controller
	{
		private readonly IProductRepository _repository;// Khai báo phụ thuộc là interface

		// Constructor Injection: IoC Container sẽ tự động tiêm FakeProductRepository vào đây
		// vì nó đã được đăng ký cho IProductRepository trong Program.cs
		public ProductController(IProductRepository repository)
		{
			_repository = repository;
		}
		// Ví dụ 1: Convention-based Routing (không có [Route] attribute ở đây)
		// Sẽ khớp với /Product/List hoặc /Product (nếu List là action mặc định của ProductController)
		public IActionResult List(string? category = null) // Tham số category để lọc, có thể null
		{
			var products = _repository.Products.Where(p => category == null || p.Category == category).ToList();
			ViewBag.CurrentCategory = category ?? "Tất cả sản phẩm";
			return View(products);
		}

		// Ví dụ 2: Action này sẽ được gọi bởi route "product_by_category" trong Program.cs
		// public IActionResult ListByCategory(string category) { /* Logic tương tự List(category)*/ }
		// (Chúng ta có thể gộp logic vào một Action `List` duy nhất như trên)

		// Ví dụ 3: Attribute Routing cho chi tiết sản phẩm
		// Sẽ khớp với /product/chi-tiet/{id}
		// [Route("product/chi-tiet/{id:int}")] // Nếu không có tiền tố Controller Route

		[Route("chi-tiet/{id:int}")] // Nếu có [Route("product")] ở cấp Controller
		public IActionResult Details(int id)
		{
			var product = _repository.Products.FirstOrDefault(p =>
			p.ProductID == id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}
	}
}
