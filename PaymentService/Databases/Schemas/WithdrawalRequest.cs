using System.ComponentModel.DataAnnotations;
using PaymentService.Commons.Schemas;
using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class WithdrawalRequest
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }

        public Guid BankAccountId { get; set; }

        [MaxLength(1000)]
        public string Note { get; set; }

        public RequestStatus Status { get; set; }

        public DateTimeOffset RequestedAt { get; set; }

        public DateTimeOffset? ApprovedAt { get; set; }

        public int ApprovedBy { get; set; }

        public CreatorInfo CreatorInfo { get; set; }

        public virtual BankAccount BankAccount { get; set; }

        public virtual TeacherEarning TeacherEarning { get; set; }
    }
}