using SportsStoreWebApp.Domain.Models;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;
using SportsStoreWebApp.Configurations;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using SportsStore.Infrastructure;
using SportsStore.Infrastructure.Data;
using SportsStore.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
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

			// Đăng ký IProductRepository và FakeProductRepository vào DI container
			// AddScoped: Một instance mới của repository được tạo cho mỗi request HTTP

			//builder.Services.AddScoped<IProductRepository, FakeProductRepository>();
			builder.Services.AddScoped<IProductRepository, EFProductRepository>();

			// Đăng ký PagingSettings để có thể inject IOptions<PagingSettings> vào Controller
			builder.Services.Configure<PagingSettings>(builder.Configuration.GetSection("PagingSettings"));
			builder.Services.AddDistributedMemoryCache();// Cần cho trạng thái phiên
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			// Để demo [FromServices] cho ILogger
			builder.Services.AddLogging(); // Kích hoạt logging service
			builder.Services.AddHttpContextAccessor();

			builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("SportsStoreConnection")));

			var app = builder.Build();
			// Đăng ký Middleware tùy chỉnh đầu tiên trong pipeline
			app.UseMiddleware<SportsStoreWebApp.Middleware.RequestLoggerMiddleware>();

			//fix lỗi tiếng việt
			Console.OutputEncoding=System.Text.Encoding.Unicode;
			Console.InputEncoding=System.Text.Encoding.Unicode;

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseSession();//Quan trọng: sau UseRouting() và trước MapControllers()

			app.UseAuthorization();

			// --- Các tuyến đường (Routes) cho phân trang và lọc danh mục ---
			// Quan trọng: Thứ tự của các tuyến đường rất quan trọng.
			// Các tuyến cụ thể hơn (có nhiều tham số hoặc định dạng chặt chẽ hơn)
			// nên được định nghĩa TRƯỚC các tuyến đường tổng quát hơn.

			// 1. Tuyến đường cho phân trang CÓ DANH MỤC: Ví dụ: /Bong%20da/Page2
			// - Bắt tham số {category} (chuỗi) và {productPage} (số nguyên)
			app.MapControllerRoute(
			name: "category_page", // Tên route này để bạn có thể tham chiếu nếu cần
			pattern: "{category}/Page{productPage:int}", // Mẫu URL: {category} (tên danh mục) / Page{ productPage} (số trang)
			defaults: new { Controller = "Product", action = "List" } //Controller và Action mặc định
			);

			// 2. Tuyến đường cho phân trang KHÔNG CÓ DANH MỤC (chỉ trang): Ví dụ:/ Page2
			// - Chỉ bắt tham số {productPage} (số nguyên)
			app.MapControllerRoute(
			name: "pagination",
			pattern: "Page{productPage:int}", // Mẫu URL: Page{productPage}
			defaults: new { Controller = "Product", action = "List" }
			);

			// 3. Tuyến đường cho LỌC THEO DANH MỤC (trang đầu tiên): Ví dụ:/ Bong % 20da
			// - Bắt tham số {category} (chuỗi)
			app.MapControllerRoute(
			name: "category",
			pattern: "{category}", // Mẫu URL: {category} (tên danh mục)
			defaults: new{ Controller = "Product", action = "List", productPage= 1} // Mặc định là trang 1
			);

			// 4. Tuyến đường MẶC ĐỊNH (tổng quát nhất): Ví dụ: /, /Product/List,/ Product / Details / 1
			// - Bắt tham số {controller}, {action}, và {id} (tùy chọn)
			app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Product}/{action=List}/{id?}"); // Đặt Product / List làm trang mặc định khi truy cập root URL

			Console.WriteLine($"Môi trường hiện tại: {app.Environment.EnvironmentName}");
			app.Run();
		}
	}
}
