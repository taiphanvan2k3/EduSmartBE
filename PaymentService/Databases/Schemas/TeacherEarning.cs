namespace PaymentService.Databases.Schemas
{
    public class TeacherEarning
    {
        public int UserId { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal TotalWithdrawn { get; set; }

        public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = [];
    }
}