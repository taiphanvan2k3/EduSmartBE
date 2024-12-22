using PaymentService.Enumerations;

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
    }
}