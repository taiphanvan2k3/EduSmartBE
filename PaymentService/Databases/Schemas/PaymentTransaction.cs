using System.ComponentModel.DataAnnotations;
using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; }

        [MaxLength(12)] // Ví dụ SEP123456789
        public string Code { get; set; }

        public int UserId { get; set; }

        public CreatorInfo CreatorInfo { get; set; }

        public int ReceiverId { get; set; }

        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }

        [MaxLength(1000)]
        public string Error { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? CompletedAt { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public TransactionType TransactionType { get; set; }

        public OrderStatus OrderStatus { get; set; }

        // Thông tin liên quan đến giao dịch
        // Lưu dạng JSON vào cột này
        public string RelatedInformation { get; set; }
    }

    public class CreatorInfo
    {
        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}