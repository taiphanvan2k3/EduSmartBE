using AuthService.Enumerations;
using AuthService.Services.Cache;
using AuthService.Services.Otp.Schemas.Wrappers;

namespace AuthService.Commons.Helpers
{
    public static class Utils
    {
        public static string GetDefaultAvatarUrl(string avatarUrl, string fullName, string username = "")
        {
            fullName = fullName.Trim();
            username = username.Trim();

            if (string.IsNullOrEmpty(fullName))
            {
                if (string.IsNullOrEmpty(username))
                {
                    fullName = username;
                }
                else
                {
                    fullName = "Unknown Name";
                }
            }

            return string.IsNullOrEmpty(avatarUrl)
                ? $"https://ui-avatars.com/api/?name={fullName}&size=128&background=random"
                : avatarUrl;
        }

        public static string GenerateOtp()
        {
            // Tạo mã OTP 6 chữ số ngẫu nhiên
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static (string, Type) GetOtpCacheKey(string email, OtpType otpType)
        {
            return otpType switch
            {
                OtpType.ResetPassword => (CacheKeyManager.GetResetPasswordKey(email), typeof(ResetPasswordWrapper)),
                OtpType.DeleteAccount => (CacheKeyManager.GetConfirmDeleteAccountKey(email), typeof(OtpWrapperBase)),
                _ => throw new ArgumentOutOfRangeException(nameof(otpType), otpType, null)
            };
        }
    }
}