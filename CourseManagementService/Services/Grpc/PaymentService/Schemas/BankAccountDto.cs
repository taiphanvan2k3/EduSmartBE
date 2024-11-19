namespace CourseManagementService.Services.Grpc.PaymentService.Schemas
{
    public class BankAccountDto
    {
        public string Bin { get; set; }

        public string BankName { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }
    }
}