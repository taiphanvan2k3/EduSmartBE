namespace PaymentService.Services.Sepay.Schemas
{
    public class PaymentQRData
    {
        public string Bin { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public decimal Amount { get; set; }

        public string TransactionOrder { get; set; }
    }
}