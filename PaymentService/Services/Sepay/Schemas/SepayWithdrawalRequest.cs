using System.Text.RegularExpressions;

namespace PaymentService.Services.Sepay.Schemas
{
    public class SepayWithdrawalRequest
    {
        public long Id { get; set; }

        public string Gateway { get; set; }

        public string TransactionDate { get; set; }

        public string AccountNumber { get; set; }

        public string Code { get; set; }

        public string Content { get; set; }

        public string PaymentContent
        {
            get
            {
                // Ví dụ như QR - SEP123456 hay IBFT - SEP123456 hay SEP123456 thì cắt bỏ phần QR - hoặc IBFT - để lấy SEP123456
                string pattern = @"\SEVQR\d+\b";
                var match = Regex.Match(Content, pattern);

                return match.Success ? match.Value : null;
            }
        }

        public string TransferType { get; set; }

        public decimal TransferAmount { get; set; }

        public string SubAccount { get; set; }

        public string ReferenceCode { get; set; }

        public string Description { get; set; }
    }
}