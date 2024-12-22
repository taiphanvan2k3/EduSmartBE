using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PaymentService.Enumerations;

namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRequestPost
    {
        public decimal Amount { get; set; }

        [EnumDataType(typeof(CurrencyType))]
        [JsonConverter(typeof(JsonStringEnumConverter))] // Cho phép gửi request dạng string
        public CurrencyType CurrencyType { get; set; }

        public Guid BankAccountId { get; set; }
    }
}