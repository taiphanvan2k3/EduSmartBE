namespace PaymentService.Services.Banks.Schemas
{
    public class BankData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Bin { get; set; }

        public string LogoUrl { get; set; }
    }
}