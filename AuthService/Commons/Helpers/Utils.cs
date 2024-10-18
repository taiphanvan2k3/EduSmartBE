using System.Security.Cryptography;
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
            // Tạo mã OTP 6 chữ số ngẫu nhiên an toàn bằng RandomNumberGenerator
            byte[] randomNumber = new byte[4];
            using var rng = RandomNumberGenerator.Create();

            // Tọ mã ngẫu nhiên (32 bit) cho mã OTP -> Phạm vi số nguyên từ -2,147,483,648 đến 2,147,483,647
            rng.GetBytes(randomNumber);

            // Chuyển mảng byte thành số nguyên và tạo mã OTP từ 100000 đến 999999
            int otp = Math.Abs(BitConverter.ToInt32(randomNumber, 0) % 900000) + 100000;
            return otp.ToString();
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