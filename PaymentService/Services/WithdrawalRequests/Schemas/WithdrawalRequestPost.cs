namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRequestPost
    {
        public decimal Amount { get; set; }

        public Guid BankAccountId { get; set; }
    }
}