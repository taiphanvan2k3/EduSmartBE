using System.Text.Json;

namespace PaymentService.Commons.Helpers
{
    public class JsonSerializerUtils
    {
        private static readonly JsonSerializerOptions s_readOptions = new()
        {
            // Khoá không phân biệt chữ hoa chữ thường
            PropertyNameCaseInsensitive = true,
        };

        private static readonly JsonSerializerOptions s_writeOptions = new()
        {
            // Lưu dạng camelCase
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, s_writeOptions);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, s_readOptions);
        }
    }
}