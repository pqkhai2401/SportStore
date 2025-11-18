using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using SportsStoreWebApp.Domain.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportsStoreWebApp.Controllers
{
	// Giả sử bạn có IProductRepository được tiêm ở đây
	public class AdminController : Controller
	{
		private readonly IProductRepository _repository;
		private readonly ILogger<AdminController> _logger;
		public AdminController(IProductRepository repository,
		ILogger<AdminController> logger)
		{
			_repository = repository;
			_logger = logger;
		}
		// Action để hiển thị form tạo/chỉnh sửa sản phẩm
		public IActionResult Edit(int productId = 0)
		{
			Product? product = productId == 0 ? new Product() : _repository.Products.FirstOrDefault(p => p.ProductID == productId);
			if (product == null && productId != 0)
			{
				_logger.LogWarning("Không tìm thấy sản phẩm có ID {ProductID} để chỉnh sửa.", productId);
				return NotFound();
			}
			return View(product);
		}
		// Action xử lý POST khi người dùng gửi form
		[HttpPost]
		[ValidateAntiForgeryToken] // Quan trọng cho bảo mật
		public IActionResult Edit(Product product)
		{
			if (ModelState.IsValid) // Kiểm tra hợp lệ dựa trên Data Annotations
			{
				// Logic lưu sản phẩm vào repository (chưa triển khai thực sự)
				// Ví dụ: _repository.SaveProduct(product);
				_logger.LogInformation("Dữ liệu sản phẩm cho '{ProductName}'hợp lệ.Sẵn sàng để lưu.", product.Name);
				TempData["message"] = $"{product.Name} đã được lưu thành công!";
				return RedirectToAction("List"); // Hoặc Admin Index
			}
			else
			{
				// Có lỗi validation, hiển thị lại form với các thông báo lỗi
				_logger.LogWarning("Dữ liệu sản phẩm cho '{ProductName}' không hợp lệ.Lỗi xác thực: { Errors}", product.Name,string.Join("; ", ModelState.Values.SelectMany(v =>
					v.Errors).Select(e => e.ErrorMessage)));
				return View(product); // Trả về cùng View với Model có lỗi
			}
		}
	}
}
