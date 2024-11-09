namespace PaymentService.Services.Banks.Schemas
{
    public class BankDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string SwiftCode { get; set; }

        public string LogoUrl { get; set; }
    }
}