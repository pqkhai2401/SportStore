using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;
using SportsStoreWebApp.Domain.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SportsStoreWebApp.Configurations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Reflection;
using System;
using System.Net.NetworkInformation;
using SportsStore.Domain.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace SportsStoreWebApp.Controllers
{
	// Áp dụng một route tiền tố cho toàn bộ Controller nếu muốn dùngAttribute Routing mạnh mẽ
	// [Route("san-pham")] // Ví dụ: mọi action sẽ bắt đầu bằng /cua-hang/
	public class ProductController : Controller
	{
		private readonly IProductRepository _repository;// Khai báo phụ thuộc là interface
		private readonly ILogger<ProductController> _logger;
		private readonly PagingSettings _pagingSettings; // Khai báo thuộc tính để lưu cấu hình phân trang
		public int PageSize = 4; // Kích thước trang

		// Hàm tạo (constructor) của Controller
		// ASP.NET Core sẽ tự động tiêm (inject) các dependency vào đây:
		// - IProductRepository: Để truy cập dữ liệu sản phẩm
		// - ILogger<ProductController>: Để ghi log
		// - IOptions<PagingSettings>: Để đọc cấu hình phân trang từ appsettings.json

		// Constructor Injection: IoC Container sẽ tự động tiêm FakeProductRepository vào đây
		// vì nó đã được đăng ký cho IProductRepository trong Program.cs
		public ProductController(IProductRepository repository,ILogger<ProductController> logger,IOptions<PagingSettings> pagingSettings)
		{
			_repository = repository;
			_logger = logger;
			_pagingSettings = pagingSettings.Value; // Lấy đối tượng PagingSettings từ IOptions
			_logger.LogInformation("ProductController da duoc tao."); // Log khi Controller được tạo
		}

		// Hành động List để hiển thị danh sách sản phẩm với phân trang và lọc
		// category: Tham số tùy chọn để lọc theo danh mục (mặc định là null)
		// productPage: Tham số tùy chọn cho số trang hiện tại (mặc định là 1)


		public async Task<ViewResult> List(string category, int productPage = 1)
		{
			var productsQuery = _repository.Products
			.Where(p => category == null ||
			p.Category == category);
			var products = await productsQuery
			.OrderBy(p => p.ProductID)
			.Skip((productPage - 1) * PageSize)
			.Take(PageSize)
			.ToListAsync(); // Thực thi truy vấn
			var totalItems = await productsQuery.CountAsync(); // Đếm tổng số lượng cho phân trang
			ViewBag.TotalItems = totalItems;
			ViewBag.ItemsPerPage = PageSize;
			ViewBag.CurrentPage = productPage;
			ViewBag.CurrentCategory = category; 
			return View(products);
		}

		// Hành động Details để hiển thị chi tiết một sản phẩm

		public IActionResult Details(int id)
		{
			var product = _repository.Products.FirstOrDefault(p =>p.ProductID == id);
			if (product == null)
			{
				// Ghi log cảnh báo nếu không tìm thấy sản phẩm
				_logger.LogWarning("Khong tim thay san pham voi ID:{ ProductID }.", id);
				return NotFound(); // Trả về lỗi 404 Not Found
			}
			// Ghi log thông tin khi hiển thị chi tiết sản phẩm
			_logger.LogInformation("Hien thi chi tiet san pham ID { ProductID}", id);
			return View(product);
		}
		// Tạo một Action để kiểm tra ghi log lỗi
		public IActionResult SimulateError()
		{
			try
			{
				_logger.LogWarning("Mô phỏng lỗi để kiểm tra nhật ký...");
				throw new InvalidOperationException("Đây là lỗi kiểm tra từ SimulateError action!");
			}
			catch (Exception ex) 
			{
				_logger.LogError(ex, "Đã xảy ra lỗi không mong muốn trong quá trình mô phỏng lỗi!");
			}
			return Content("Kiểm tra đầu ra console/debug của bạn để tìm nhật ký!");
		}
        public IActionResult FilterProducts(ProductFilter filter)
		{
            _logger.LogInformation("Lọc sản phẩn theo Category: {Category},MinPrice: { MinPrice}, MaxPrice: { MaxPrice}, InStockOnly: { InStock}",
			filter.Category, filter.MinPrice, filter.MaxPrice, filter.InStockOnly);
            // Logic lọc sản phẩm dựa trên filter
            var filteredProducts = _repository.Products;
            if (!string.IsNullOrEmpty(filter.Category))
            {
                filteredProducts = filteredProducts.Where(p => p.Category ==
                filter.Category);
            }
            if (filter.MinPrice.HasValue)
			{
                filteredProducts = filteredProducts.Where(p => p.Price >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <=
                filter.MaxPrice.Value);
            }
            // Nếu InStockOnly = true, thì lọc thêm điều kiện này
            // if (filter.InStockOnly) { filteredProducts = filteredProducts.Where(p => p.IsInStock());
            return View("List", filteredProducts.ToList()); // Tái sử dụng View List
        }
		// Action Edit (GET): Hiển thị form chỉnh sửa sản phẩm
		// Action Edit (để thử Create, sẽ hoàn thiện Update sau)
		public async Task<IActionResult> Edit(int productId = 0)
		{
			Product? product = productId == 0 ? new Product() : await
			_repository.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
			if (product == null && productId != 0)
			{
				return NotFound();
			}
			return View(product);
		}
		[
		HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Product product)
		{
			if (ModelState.IsValid)
			{
				await _repository.SaveProduct(product); // Gọi SaveProduct để thêm(hoặc cập nhật)
				TempData["message"] = $"{product.Name} đã được lưu thành công.";
				return RedirectToAction("List");
			}
			else
			{
				return View(product);
			}
		}

		// Action Create: Hiển thị form tạo sản phẩm mới (tương tự Edit nhưng ID = 0)
		public ViewResult Create()
		{
			return View("Edit", new Product()); // Sử dụng lại View Edit với một Product trống
		}
			// Action Delete: Giả lập xóa sản phẩm (chỉ để hoàn thiện logic cơ bản)
			[HttpPost]
			public IActionResult Delete(int productId)
			{
				Product? productToDelete =
				_repository.Products.FirstOrDefault(p => p.ProductID == productId);
				if (productToDelete != null)
				{
					// FakeProductRepository không có Remove, nên chỉ log
					_logger.LogInformation("Sản phẩm '{ProductName}' (ID:{ ProductId}) được đánh dấu để xóa(thực tế không bị xóa trong FakeRepository).", productToDelete.Name, productToDelete.ProductID);
				TempData["message"] = $"{productToDelete.Name} đã đượcđánh dấu xóa!";
				}
				else
				{
					TempData["message"] = $"Sản phẩm có ID {productId} không tồn tại để xóa.";
				}
				return RedirectToAction("List");
			}

	}
	
}
