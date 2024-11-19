namespace CourseManagementService.Services.Grpc.PaymentService.Schemas
{
    public class PaymentTransactionData
    {
        public string TransactionCode { get; set; }

        public int UserId { get; set; }

        public double Amount { get; set; }

        public string CurrencyCode { get; set; }

        public string RelatedInfo { get; set; }
    }
}