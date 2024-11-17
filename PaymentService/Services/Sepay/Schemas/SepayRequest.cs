namespace PaymentService.Services.Sepay.Schemas
{
    public class SepayRequest
    {
        public long Id { get; set; }

        public string Gateway { get; set; }

        public string TransactionDate { get; set; }

        public string AccountNumber { get; set; }

        public string SubAccount { get; set; }

        public string Content { get; set; }

        public TransactionType TransactionType { get; set; }

        public decimal TransferAmount { get; set; }
    }
}