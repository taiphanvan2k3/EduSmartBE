namespace AuthService.Services.Cache
{
    public static class CacheKeyManager
    {
        public static string GetOtpKey(string email) => $"OTP:{email}";
    }
}