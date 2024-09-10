using System.Text.Json.Serialization;

namespace AuthService.Services.Permission.Schemas.Function
{
    public class PagingInfo
    {
        [JsonPropertyName("page")]
        public int PageIndex { get; set; }

        [JsonPropertyName("size")]
        public int PageSize { get; set; }
    }
}