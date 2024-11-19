namespace CourseManagementService.Services.Grpc.PaymentService.Schemas
{
    public class CoursePaymentInfo
    {
        public string TransactionId { get; set; }

        public string CourseName { get; set; }

        public decimal PriceAtVnd { get; set; }

        public decimal PriceAtUsd { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string BankName { get; set; }

        public string QRCode { get; set; }
    }
}