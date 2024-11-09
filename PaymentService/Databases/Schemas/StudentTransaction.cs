using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class StudentTransaction
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public RequestStatus Status { get; set; }
    }
}