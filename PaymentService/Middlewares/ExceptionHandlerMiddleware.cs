using PaymentService.Commons.Helpers;

namespace PaymentService.Middlewares
{
    public class ExceptionHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                // Kiểm tra xem response đã được set header hay chưa
                if (context.Response.HasStarted)
                {
                    return;
                }

                // Xử lý lỗi và trả về response chung
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = ErrorResponseHelper.GetContentOfInternalServerResponse(e);
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}