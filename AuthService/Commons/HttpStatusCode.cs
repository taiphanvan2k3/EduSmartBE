namespace AuthService.Commons
{
    public class HttpStatusCode
    {
        public static readonly int OK = 200;

        public static readonly int CREATED = 201;

        public static readonly int BAD_REQUEST = 400;

        public static readonly int UNAUTHORIZED = 401;

        public static readonly int FORBIDDEN = 403;

        public static readonly int NOT_FOUND = 404;

        public static readonly int METHOD_NOT_ALLOWED = 405;
        
        public static readonly int INTERNAL_SERVER_ERROR = 500;
    }
}