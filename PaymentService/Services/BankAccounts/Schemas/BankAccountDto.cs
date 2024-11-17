namespace PaymentService.Services.BankAccounts.Schemas
{
    public class BankAccountDto
    {
        public Guid Id { get; set; }

        public int BankId  { get; set; }

        public int UserId { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public bool IsPrimary { get; set; }        
    }
}