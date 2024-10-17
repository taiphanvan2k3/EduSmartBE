namespace AuthService.Services.Cache
{
    public static class CacheKeyManager
    {
        public static string GetResetPasswordKey(string email) => $"ResetPassword:{email}";

        public static string GetConfirmDeleteAccountKey(string email) => $"ConfirmDeleteAccount:{email}";
    }
}