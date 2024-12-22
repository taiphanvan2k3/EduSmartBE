using System.Text.Json.Serialization;
using PaymentService.Commons.Schemas;

namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRequestForAdmin : WithdrawalRequestDto
    {
        [JsonPropertyOrder(-1)]
        public CreatorInfo CreatorInfo { get; set; }

        public decimal AmountInBaseCurrency { get; set; }

        public string BaseCurrency { get; set; } = "VND";
    }
}