using System.ComponentModel.DataAnnotations;

namespace PaymentService.Databases.Schemas
{
    public class BankAccount : BaseEntity
    {
        public Guid Id { get; set; }

        public int BankId { get; set; }

        public int UserId { get; set; }

        [MaxLength(20)]
        public string AccountNumber { get; set; }

        [MaxLength(255)]
        public string AccountName { get; set; }

        public bool IsPrimary { get; set; }

        public virtual Bank Bank { get; set; }

        public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = [];
    }
}