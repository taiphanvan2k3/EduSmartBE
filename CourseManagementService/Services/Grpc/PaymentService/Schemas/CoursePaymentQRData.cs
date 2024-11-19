namespace CourseManagementService.Services.Grpc.PaymentService.Schemas
{
    public class CoursePaymentQRData
    {
        public BankAccountDto AdminAccount { get; set; }

        public decimal Amount { get; set; }

        public string TransactionOrder { get; set; }
    }
}