using SportsStoreWebApp.Domain.Models;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;
namespace SportsStoreWebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			// Đăng ký dịch vụ Product Repository với vòng đời Scoped
			// Mỗi yêu cầu HTTP sẽ nhận một thể hiện mới của FakeProductRepository
			builder.Services.AddScoped<IProductRepository, FakeProductRepository>();

			
			builder.Services.AddControllersWithViews(); //Đăng ký các dịch vụ cần thiết cho MVC

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

			// *** Route cụ thể hơn: Ví dụ cho các URL có cấu trúc rõ ràng cho sảnphẩm theo danh mục ***
			app.MapControllerRoute(
			name: "product_by_category",
			pattern: "san-pham/danh-muc/{category}", // URL sẽ là /san-pham/danhmuc/bong-da
			defaults: new { controller = "Product", action = "ListByCategory" }
			);
			// *** Route mặc định (tổng quát hơn, nên đặt cuối cùng) ***
			app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");


			app.Run();
		}
	}
}
