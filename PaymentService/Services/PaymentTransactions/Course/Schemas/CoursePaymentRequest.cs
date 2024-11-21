namespace PaymentService.Services.PaymentTransactions.Course.Schemas
{
    public class CoursePaymentRequest
    {
        public Guid TransactionId { get; set; }

        public Guid CourseId { get; set; }

        public int TeacherId { get; set; }

        public int BuyerId { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset PaymentDate { get; set; }
    }
}