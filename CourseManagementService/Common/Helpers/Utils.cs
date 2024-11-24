using System.Security.Cryptography;
using System.Text;
using CourseManagementService.Services.Grpc.PaymentService.Schemas;
using Xabe.FFmpeg;

namespace CourseManagementService.Common.Helpers
{
    public class Utils
    {
        public static string GetEnumName(Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        public static string ConvertStringToBase64(string input)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static void CreateUploadFolderIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static async Task<string> SaveFileLocally(string folderPath, string fileName, IFormFile file)
        {
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return filePath;
        }

        public static string ExtractPublicId(string url)
        {
            try
            {
                // Example http://res.cloudinary.com/da1aqhx1g/image/upload/v1729529214/q0joipmzjgfxdkugh9tj.png
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                var fileName = segments.Last(); // e.g., "q0joipmzjgfxdkugh9tj.png"
                var publicId = Path.GetFileNameWithoutExtension(fileName); // "q0joipmzjgfxdkugh9tj"
                return publicId;
            }
            catch
            {
                return "";
            }
        }

        public static async Task<int> GetDurationOfVideo(string filePath)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(filePath);
            var totalSeconds = mediaInfo.Duration.TotalSeconds;
            return (int)totalSeconds;
        }

        public static string ConvertSecondsToDuration(long totalSeconds)
        {
            var timeSpan = TimeSpan.FromSeconds(totalSeconds);
            if (totalSeconds < 3600)
            {
                return timeSpan.ToString(@"mm\:ss");
            }

            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        public static string GenerateRandomString(int length = 6)
        {
            // Tạo mã OTP 6 chữ số ngẫu nhiên an toàn bằng RandomNumberGenerator
            byte[] randomNumber = new byte[length];
            using var rng = RandomNumberGenerator.Create();

            // Tọ mã ngẫu nhiên (32 bit) cho mã OTP -> Phạm vi số nguyên từ -2,147,483,648 đến 2,147,483,647
            rng.GetBytes(randomNumber);

            // Chuyển mảng byte thành số nguyên và tạo mã OTP từ 100000 đến 999999
            // int otp = Math.Abs(BitConverter.ToInt32(randomNumber, 0) % 900000) + 100000;
            int otp = Math.Abs(BitConverter.ToInt32(randomNumber, 0) % (9 * (int)Math.Pow(10, length - 1)))
                + (int)Math.Pow(10, length - 1);
            return otp.ToString();
        }

        public static string GenerateQRCodeForCoursePayment(Guid courseId, CoursePaymentQRData qRData)
        {
            return $"https://img.vietqr.io/image/{qRData.AdminAccount.Bin}-{qRData.AdminAccount.AccountNumber}-compact2.png?amount={qRData.Amount}&addInfo={qRData.TransactionOrder}&accountName={qRData.AdminAccount.AccountName}";
        }

        public static decimal ExchangeCurrency(decimal amount, string fromCurrency, string toCurrency, decimal usdToVndRate)
        {
            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            if (fromCurrency == "USD")
            {
                return amount * usdToVndRate;
            }

            if (toCurrency == "USD")
            {
                return amount / usdToVndRate;
            }

            throw new Exception("Unsupported currency exchange");
        }
    }
}