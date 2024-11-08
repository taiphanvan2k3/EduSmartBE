using System.Text.Json;
using AuthService.Commons.Helpers;

namespace AuthService.Middlewares
{
    public class CustomUnauthorizedMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            // Kiểm tra xem response đã được set header hay chưa
            if (context.Response.HasStarted)
            {
                return;
            }

            // Sau khi xử lý request, kiểm tra response status code
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var response = new ErrorResponse
                {
                    StatusCode = 401,
                    Error = "Unauthorized",
                    Message = "You need to be authenticated to access this resource.",
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = new ErrorResponse
                {
                    StatusCode = 403,
                    Error = "Forbidden",
                    Message = "You are not authorized to access this resource.",
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}