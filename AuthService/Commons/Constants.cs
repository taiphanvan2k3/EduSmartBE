namespace AuthService.Commons
{
    public class Constants
    {
        public static readonly string SERVICE_NAME = (typeof(Constants).Namespace ?? "AuthService").Split('.')[0];
        public static readonly string CONNECTION_STRING = "Host=localhost;Port=5432;Database=auth_service;Username=postgres;Password=postgres";
    }
}