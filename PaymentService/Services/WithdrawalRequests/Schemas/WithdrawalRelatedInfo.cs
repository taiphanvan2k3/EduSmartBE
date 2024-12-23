namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRelatedInfo
    {
        public Guid WithdrawalRequestId { get; set; }

        public string QRCode { get; set; }
    }
}