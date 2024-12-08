namespace PaymentService.Commons
{
    public static class Constants
    {
        public static readonly string ADMIN_EMAIL = "teampblpro@gmail.com";

        public static readonly string DEFAULT_ADMIN_PASSWORD = "Admin@123";

        public static readonly string SERVICE_NAME = (typeof(Constants).Namespace ?? "PaymentService").Split('.')[0];

        public static readonly string CONNECTION_STRING = "Host=edu-smart.postgres.database.azure.com;Database=EduSmart.PaymentService;Username=pbl6duter;Password=Edusmartk21;SSL Mode=Require;Trust Server Certificate=true";

        public static readonly int EXCHANGE_RATE_USD_TO_VND = 25397;

        public static readonly int FREE_MAXIMUM_STORAGE_AMOUNT = 500 * 1024 * 1024; // 500MB
    }
}