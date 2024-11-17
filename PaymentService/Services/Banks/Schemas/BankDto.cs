using System.Text.Json.Serialization;

namespace PaymentService.Services.Banks.Schemas
{
    public class BankDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Bin { get; set; }

        [JsonPropertyName("logo")]
        public string LogoURL { get; set; }
    }
}