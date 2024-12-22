using System.Text.Json.Serialization;
using PaymentService.Enumerations;

namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRequestDto
    {
        [JsonPropertyOrder(-3)]
        public Guid Id { get; set; }

        [JsonPropertyOrder(-2)]
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public Guid BankAccountId { get; set; }

        public string BankName { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankAccountName { get; set; }

        public RequestStatus Status { get; set; }

        public DateTimeOffset RequestedAt { get; set; }

        public DateTimeOffset? ApprovedAt { get; set; }
    }
}