using System.Security.Cryptography;
using PaymentService.Enumerations;
using PaymentService.Services.Sepay.Schemas;

namespace PaymentService.Commons.Helpers
{
    public class Utils
    {
        /// <summary>
        /// Convert the amount to the base currency (VND)
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="currency">Currency of the amount</param>
        /// <returns></returns>
        public static decimal ConvertToBaseCurrency(decimal amount, CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.USD => amount * Constants.EXCHANGE_RATE_USD_TO_VND,
                CurrencyType.VND => amount,
                _ => throw new NotImplementedException()
            };
        }

        public static DateTimeOffset? ToUniversalTime(DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset?.ToUniversalTime();
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

        public static string GenerateQRCode(PaymentQRData qRData)
        {
            return $"https://img.vietqr.io/image/{qRData.Bin}-{qRData.AccountNumber}-compact2.png?amount={qRData.Amount}&addInfo={qRData.TransactionOrder}&accountName={qRData.AccountName}";
        }
    }
}