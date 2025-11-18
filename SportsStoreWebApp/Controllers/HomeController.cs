using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Đảm bảo đã import namespace này
using System; // Cần cho InvalidOperationException
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Configurations;
using Microsoft.Extensions.Options;

namespace SportsStoreWebApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IProductRepository _repository;
		private readonly PagingSettings _pagingSettings; // Khai báo thuộc tính để lưu cấu hình phân trang

		public HomeController(IProductRepository repository, ILogger<HomeController> logger, IOptions<PagingSettings> pagingSettings)
		{
			_repository = repository;
			_logger = logger;
			_pagingSettings = pagingSettings.Value; // Lấy đối tượng PagingSettings từ IOptions
		}

		public IActionResult Index(string? category = null, int productPage =1)
		{
			// Ghi log thông tin về yêu cầu truy cập trang sản phẩm
			_logger.LogInformation("Yêu cầu danh sách sản phẩm. Danh mục:{ Category}, Trang: { Page}", category, productPage);
			// Lấy số sản phẩm trên mỗi trang từ cấu hình PagingSettings
			int itemsPerPage = _pagingSettings.ItemsPerPage;
			// int maxPagesToShow = _pagingSettings.MaxPagesToShow; // Có thể dùng sau nếu muốn giới hạn số nút trang hiển thị
			// Lọc sản phẩm theo danh mục (nếu category không null hoặc rỗng)
			// Sau đó sắp xếp và thực hiện phân trang (Skip/Take)
			var productsQuery = _repository.Products
			.Where(p => category == null
			|| p.Category == category);
						var products = productsQuery
						.OrderBy(p => p.ProductID) //Quan trọng: Sắp xếp trước khi Skip / Take để đảm bảo phân trang đúng thứ tự
			.Skip((productPage - 1) * itemsPerPage) // Bỏ qua các sản phẩm của các trang trước đó
			.Take(itemsPerPage) // Lấy số sản phẩm bằng ItemsPerPage cho trang hiện tại
			.ToList(); // Chuyển kết quả sang List để truyền cho View
			// Chuẩn bị dữ liệu cần thiết cho View thông qua ViewBag
			ViewBag.Categories = _repository.Products
			.Select(p => p.Category)
			.Distinct()
			.OrderBy(c => c)
			.ToList();
						ViewBag.CurrentCategory = category ?? "Tất cả sản phẩm"; //Danh mục hiện tại
			ViewBag.CurrentPage = productPage; // Trang hiện tại
						ViewBag.TotalItems = productsQuery.Count(); // Tổng số sản phẩm SAU KHI lọc, nhưng TRƯỚC KHI phân trang
			ViewBag.ItemsPerPage = itemsPerPage; // Số sản phẩm trên mỗi trang
			// Ghi log thông tin về số lượng sản phẩm được trả về
			//_logger.LogInformation("Trả về {ProductCount} sản phẩm cho trang { Page}. Tổng số sản phẩm: { TotalItems}", products.Count,productPage, ViewBag.TotalItems);
			return View(products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None,
		NoStore = true)]
		public IActionResult Error()
		{
            // Lấy thông tin lỗi
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error != null)
			{
                // Sử dụng _logger instance để ghi log lỗi
                _logger.LogError(exceptionHandlerPathFeature.Error, "Một ngoại lệ chưa được xử lý đã xảy ra tại { Path}", exceptionHandlerPathFeature.Path);

            }
            else
            {
                // Ghi log nếu không có thông tin lỗi cụ thể từ exception handler
				_logger.LogWarning("Đã gọi hành động lỗi nhưng không tìm thấy tính năng ngoại lệ cụ thể nào.");
			}
            // Truyền RequestId để người dùng có thể báo cáo lỗi
            ViewBag.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
		public IActionResult AboutUs()
		{
			_logger.LogInformation("Yêu cầu trang Giới thiệu.");
			// Có thể truyền một thông điệp đơn giản
			ViewBag.Message = "Đây là trang giới thiệu về chúng tôi.";
			return View(); // Trả về Views/Home/AboutUs.cshtml (cần tạo)
		}
		// Ví dụ trả về JsonResult
		public IActionResult GetServerTime()
		{
			_logger.LogInformation("Đã yêu cầu thời gian máy chủ.");
			return Json(new
			{
				Time = DateTime.Now.ToString("HH:mm:ss"),
				Date
			= DateTime.Now.ToShortDateString()
			});
		}
		// Ví dụ chuyển hướng
		public IActionResult GoToProductList()
		{
			_logger.LogInformation("Đang chuyển hướng đến danh sách sản phẩm.");
		return RedirectToAction("List", "Product"); // Chuyển hướng đến ProductController.List
		}
		// Action để giả lập lỗi 500
		public IActionResult SimulateFatalError()
        {
            throw new InvalidOperationException("Đây là một ngoại lệ được cố ý đưa ra để kiểm tra cách xử lý lỗi!!");
        }
    }
}
