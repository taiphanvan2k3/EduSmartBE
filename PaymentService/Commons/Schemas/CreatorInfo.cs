using System.Text.Json.Serialization;

namespace PaymentService.Commons.Schemas
{
    public class CreatorInfo
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("avatarURL")]
        public string AvatarURL { get; set; }
    }
}