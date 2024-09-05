namespace AuthService.Commons
{
    public static class Constants
    {
        public static readonly string SERVICE_NAME = (typeof(Constants).Namespace ?? "AuthService").Split('.')[0];
        
        public static readonly string CONNECTION_STRING = "Host=edu-smart.postgres.database.azure.com;Database=EduSmart.AuthService;Username=pbl6duter;Password=Edusmartk21;SSL Mode=Require;Trust Server Certificate=true";
    }
}