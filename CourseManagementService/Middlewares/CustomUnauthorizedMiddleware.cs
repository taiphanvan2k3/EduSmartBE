using System.Text.Json;

namespace CourseManagementService.Middlewares
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
                var response = new
                {
                    message = "You need to be authenticated to access this resource.",
                    statusCode = 401
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    message = "You are not authorized to access this resource.",
                    statusCode = 403
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}