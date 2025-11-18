using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Models;
using System.Collections;
using System.Collections.Generic;

namespace SportsStoreWebApp.Controllers
{
	public class TestController : Controller
	{
		
		
			// 1. Binding từ Route Data hoặc Query String: /Test/Details/10 hoặc /Test/Details? id = 10
			public IActionResult Details(int id)
			{
				// int id sẽ tự động được gán giá trị 10
				ViewBag.Message = $"Sản phẩm ID: {id}"; // Dùng ViewBag để truyền dữ liệu cho View
				return View("Message"); // Trả về một View chung để hiển thị thông báo
			}
			// 2. Binding từ Query String và Route Data:/Test/List? category = Bongda & productPage = 2
			// Hoặc có thể là /Test/List/Bongda?productPage=2 nếu có route tương ứng(sẽ cấu hình sau)
			public IActionResult List(string category, int productPage = 1)
			// productPage có giá trị mặc định
			{
				ViewBag.Message = $"Danh mục: {category ?? "N/A"}, Trang:{productPage}";
				return View("Message");
			}
			// 3. Binding từ Form Data (trong trường hợp POST request từ form HTML)
			// Action này sẽ hiển thị form
			[HttpGet]
			public IActionResult CreateOrder()
			{
				return View(); // Trả về Views/Test/CreateOrder.cshtml
			}
			// Action này sẽ nhận dữ liệu từ form POST
			[HttpPost]
			public IActionResult SubmitOrder(Order order) // 'Order' là một Model class
			{
				if (ModelState.IsValid) // Kiểm tra tính hợp lệ của model (nếu có Validation)
				{
				ViewBag.Message = $"Đã nhận đơn hàng! Khách hàng:{order.CustomerName}, Sản phẩm: {order.ProductName}, Số lượng:{ order.Quantity}, Shipping Address: { order.ShippingAddress}";
				// Ở đây bạn sẽ xử lý logic lưu đơn hàng vào DB, gửi email, v.v.
				return View("Message");
				}
				else
				{
					ViewBag.Message = "Dữ liệu đơn hàng không hợp lệ. Vui lòng kiểm tra thông tin bạn nhập.";
				return View("CreateOrder", order); // Trả về lại form nếu có lỗi
				}
			}
		// 4. Binding với các Attribute cụ thể ([FromQuery], [FromRoute],[FromForm], [FromBody], [FromHeader], [FromServices])
		// Attribute Routing: api/products/{id}
		[HttpGet("api/products/{id}")]
		public IActionResult GetProductById([FromRoute] int id)
		{
			// [FromRoute] buộc ASP.NET Core phải lấy 'id' từ route data
			return Json(new { Id = id, Name = $"API Sản phẩm {id}" }); //Trả về JSON
		}
		// Demo [FromQuery]
		[HttpGet("search")]
		public IActionResult SearchProducts([FromQuery] string keyword,
		[FromQuery(Name = "max_price")] decimal maxPrice)
		{
			// [FromQuery] buộc ASP.NET Core phải lấy 'keyword' và 'max_price' từ query string
			return Json(new { Keyword = keyword, MaxPrice = maxPrice });
		}
		// Demo [FromHeader]
		[HttpGet("headers")]
		public IActionResult GetHeaders([FromHeader(Name = "User-Agent")]
			string userAgent,
		[FromHeader(Name = "AcceptLanguage")] string acceptLanguage)
		{
			// [FromHeader] buộc ASP.NET Core phải lấy dữ liệu từ header của request
			return Json(new
		{
			UserAgent = userAgent,
			AcceptLanguage = acceptLanguage});
		}
		// Demo [FromBody] (thường dùng cho API với POST/PUT request, nhận JSON/XML)
		[HttpPost("submitjson")]
		public IActionResult SubmitJsonOrder([FromBody] Order order)
		{
			// [FromBody] buộc ASP.NET Core phải lấy dữ liệu từ body của request(thường là JSON)
			if (order != null)
			{
				return Ok($"JSON Đơn hàng được nhận từ:{ order.CustomerName} cho { order.ProductName}");
			}
			return BadRequest("Thứ tự JSON không hợp lệ!");
		}

		// Demo [FromServices] (inject dịch vụ vào action method)
		// Điều này ít phổ biến hơn inject vào constructor, nhưng hữu ích cho các dịch vụ cụ thể chỉ dùng ở một action.
		public IActionResult ShowTime([FromServices] ILogger<TestController> logger)
		{
			logger.LogInformation("Hành động ShowTime được gọi tại { time}", DateTime.Now);
			return Content($"Giờ máy chủ hiện tại (từ dịch vụ):{ DateTime.Now}");
		}


	}
}
