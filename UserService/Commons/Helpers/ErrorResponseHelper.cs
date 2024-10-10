namespace UserService.Commons.Helpers
{
    public static class ErrorResponseHelper
    {
        public static ErrorResponse GetContentOfBadRequestResponse<T>(T message)
        {
            return new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Error = "Bad Request",
                Message = message
            };
        }

        public static ErrorResponse GetContentOfInternalServerResponse(string message)
        {
            return new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Error = "Internal Server Error",
                Message = message
            };
        }

        public static ErrorResponse GetContentOfInternalServerResponse(Exception exception)
        {
            return new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Error = "Internal Server Error",
                Message = exception.InnerException?.Message ?? exception.Message
            };
        }

        public static ErrorResponse GetContentOfAnyError<T>(int statusCode, string error, T message)
        {
            return new ErrorResponse
            {
                StatusCode = statusCode,
                Error = error,
                Message = message
            };
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string Error { get; set; }

        public dynamic Message { get; set; }
    }
}