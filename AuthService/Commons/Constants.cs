namespace AuthService.Commons
{
    public static class Constants
    {
        public static readonly string ADMIN_EMAIL = "teampblpro@gmail.com";

        public static readonly string DEFAULT_ADMIN_PASSWORD = "Admin@123";

        public static readonly string SERVICE_NAME = (typeof(Constants).Namespace ?? "AuthService").Split('.')[0];

        public static readonly string CONNECTION_STRING = "Host=edu-smart.postgres.database.azure.com;Database=EduSmart.AuthService;Username=pbl6duter;Password=Edusmartk21;Trust Server Certificate=true";
    }
}