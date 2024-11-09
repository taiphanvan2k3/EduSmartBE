namespace PaymentService.Databases.Schemas
{
    public class Bank : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string SwiftCode { get; set; }

        public string LogoUrl { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; } = [];
    }
}