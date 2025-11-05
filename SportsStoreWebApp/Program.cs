using SportsStoreWebApp.Domain.Models;
namespace SportsStoreWebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			var app = builder.Build();
			// Đăng ký Middleware tùy chỉnh đầu tiên trong pipeline
			app.UseMiddleware<SportsStoreWebApp.Middleware.RequestLoggerMiddleware>();

			//fix lỗi tiếng việt
			Console.OutputEncoding=System.Text.Encoding.Unicode;
			Console.InputEncoding=System.Text.Encoding.Unicode;

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			//// --- Bắt đầu phần thực hành C# cơ bản ---
			//Console.WriteLine("--- Thực hành C# cơ bản ---");
			//// Tạo danh sách sản phẩm mẫu
			//List<SportsStoreWebApp.Domain.Models.Product> sampleProducts = new
			//List<SportsStoreWebApp.Models.Product>
			//{
			//new SportsStoreWebApp.Models.Product { ProductID = 1, Name = "Bóng đá World Cup", Description = "Bóng đá chính hãng", Price = 50.00m, Category = "Bóng đá" },
			//new SportsStoreWebApp.Models.Product { ProductID = 2, Name = "Áo đấu CLB A", Description = "Áo đấu cho người hâm mộ", Price = 75.50m, Category = "Quần áo" },
			//new SportsStoreWebApp.Models.Product { ProductID = 3, Name = "Vợt Tennis Pro", Description = "Vợt chuyên nghiệp", Price = 150.00m, Category = "Tennis" },
			//new SportsStoreWebApp.Models.Product { ProductID = 4, Name = "Giày chạy bộ ABC", Description = "Giày thể thao nhẹ", Price = 99.99m, Category = "Giày" },
			//new SportsStoreWebApp.Models.Product { ProductID = 5, Name = "Bóng rổ NBA", Description = "Bóng rổ tiêu chuẩn", Price = 45.00m, Category = "Bóng rổ" }
			//};
			//Console.WriteLine("\n--- LINQ: Lọc sản phẩm có giá trên 70 ---");
			//var expensiveProducts = sampleProducts.Where(p => p.Price > 70.00m);
			//foreach (var p in expensiveProducts)
			//{
			//	Console.WriteLine($"- {p.Name} ({p.Price:C})");
			//}
			//Console.WriteLine("\n--- LINQ: Lấy sản phẩm đầu tiên thuộc danh mục 'Bóng đá' ---");
			//var firstFootballProduct = sampleProducts.FirstOrDefault(p => p.Category == "Bóng đá");
			//if (firstFootballProduct != null)
			//{
			//	Console.WriteLine($"- {firstFootballProduct.Name}");
			//}
			//else
			//{
			//	Console.WriteLine("Không tìm thấy sản phẩm bóng đá.");
			//}
			//Console.WriteLine("\n--- Async/Await: Mô phỏng thao tác bất đồng bộ ---");
			//async Task SimulateDataFetchAsync()
			//{
			//	Console.WriteLine("Đang bắt đầu lấy dữ liệu (mất 2 giây)...");
			//	await Task.Delay(2000); // Mô phỏng thao tác tốn thời gian
			//	Console.WriteLine("Đã lấy xong dữ liệu.");
			//}
			//// Gọi hàm bất đồng bộ
			// SimulateDataFetchAsync(); 
			//Console.WriteLine("--- Kết thúc thực hành C# cơ bản ---\n");
			//// --- Kết thúc phần thực hành C# cơ bản ---
			app.Run();
		}
	}
}
