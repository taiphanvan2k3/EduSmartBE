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
                return $"https://api.dicebear.com/9.x/miniavs/svg?seed={username}";
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

        public static string GenerateRandomPassword(int length = 8)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            const string numberChars = "0123456789";
            const string allChars = lowerChars + upperChars + numberChars + specialChars;

            using var rng = RandomNumberGenerator.Create();
            var result = new char[length];

            // Đảm bảo có ít nhất một ký tự viết thường, một ký tự viết hoa, và một ký tự đặc biệt
            result[0] = lowerChars[RandomNumber(rng, lowerChars.Length)];
            result[1] = upperChars[RandomNumber(rng, upperChars.Length)];
            result[2] = specialChars[RandomNumber(rng, specialChars.Length)];
            result[3] = numberChars[RandomNumber(rng, numberChars.Length)];

            // Điền các ký tự còn lại
            for (int i = 4; i < length; i++)
            {
                result[i] = allChars[RandomNumber(rng, allChars.Length)];
            }

            // Xáo trộn kết quả để các ký tự yêu cầu không ở vị trí cố định
            return new string(ShuffleArray(result, rng));
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

        private static int RandomNumber(RandomNumberGenerator rng, int maxExclusive)
        {
            var randomNumber = new byte[1];
            do
            {
                rng.GetBytes(randomNumber);
            } while (randomNumber[0] >= maxExclusive * (byte.MaxValue / maxExclusive));

            return randomNumber[0] % maxExclusive;
        }

        private static char[] ShuffleArray(char[] array, RandomNumberGenerator rng)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = RandomNumber(rng, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
            return array;
        }
    }
}