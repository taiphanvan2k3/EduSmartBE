namespace PaymentService.Databases.Schemas
{
    public class BankAccount : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid BankId  { get; set; }

        public int UserId { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public bool IsPrimary { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = [];
    }
}