namespace UserService.Commons
{
    public static class Constants
    {
        public const string CollateAsCs = "SQL_Latin1_General_CP1_CS_AS";

        public static readonly string ADMIN_EMAIL = "teampblpro@gmail.com";

        public static readonly string DEFAULT_ADMIN_PASSWORD = "Admin@123";

        public static readonly string SERVICE_NAME = (typeof(Constants).Namespace ?? "UserService").Split('.')[0];

        public static readonly string CONNECTION_STRING = "Host=edu-smart.postgres.database.azure.com;Database=EduSmart.UserService;Username=pbl6duter;Password=Edusmartk21;SSL Mode=Require;Trust Server Certificate=true";
    }
}