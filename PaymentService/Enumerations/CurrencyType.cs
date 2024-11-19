using Microsoft.EntityFrameworkCore;

namespace PaymentService.Enumerations
{
    public enum CurrencyType
    {
        [Comment("Viet Nam Dong")]
        VND = 1,

        [Comment("United States Dollar")]
        USD = 2
    }
}