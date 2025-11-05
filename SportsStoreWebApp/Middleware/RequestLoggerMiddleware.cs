using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Diagnostics; // Dùng cho Debug.WriteLine

namespace SportsStoreWebApp.Middleware
{
	public class RequestLoggerMiddleware
	{
		private readonly RequestDelegate _next; // Đại diện cho middleware tiếp theo trong pipeline
		// Constructor nhận RequestDelegate (middleware tiếp theo)
		public RequestLoggerMiddleware(RequestDelegate next)
		{
			_next = next;
		}
		// Phương thức InvokeAsync là nơi logic xử lý được thực thi
		public async Task InvokeAsync(HttpContext context)
		{
			// Logic trước khi chuyển yêu cầu đến middleware tiếp theo
			Debug.WriteLine($"---> Request đến: {context.Request.Method}{ context.Request.Path}");
		// Chuyển yêu cầu đến middleware tiếp theo trong pipeline
		await _next(context);
			// Logic sau khi yêu cầu đã được xử lý bởi các middleware sau hoặc endpoint
		Debug.WriteLine($"<--- Phản hồi cho: {context.Request.Method} { context.Request.Path} với Status: { context.Response.StatusCode}");
		}
	}
}
