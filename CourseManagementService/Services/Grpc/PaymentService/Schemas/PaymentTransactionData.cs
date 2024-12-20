using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.Grpc.PaymentService.Schemas
{
    public class PaymentTransactionData
    {
        public string TransactionCode { get; set; }

        public UserDetail CreatedBy { get; set; }

        public double Amount { get; set; }

        public string CurrencyCode { get; set; }

        public string RelatedInfo { get; set; }

        public int ReceiverId { get; set; }
    }
}