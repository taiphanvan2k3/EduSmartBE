using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class WithdrawalRequest
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public Guid BankAccountId { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public virtual BankAccount BankAccount { get; set; }
    }
}