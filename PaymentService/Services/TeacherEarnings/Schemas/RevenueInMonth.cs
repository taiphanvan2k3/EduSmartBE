using PaymentService.Enumerations;

namespace PaymentService.Services.TeacherEarnings.Schemas
{
    public class RevenueInMonth
    {
        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }

        public string Month { get; set; }
    }
}