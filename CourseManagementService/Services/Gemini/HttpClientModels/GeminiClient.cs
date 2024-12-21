using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CourseManagementService.Services.Gemini.HttpClientModels
{
    public class GeminiClient(HttpClient httpClient, ILogger<GeminiClient> logger)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<GeminiClient> _logger = logger;
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public async Task<string> GenerateContentAsync(string prompt, string newUserInput)
        {
            _logger.LogInformation("Start generating content from Gemini");
            try
            {
                var requestBody = GeminiRequestFactory.CreateRequest(prompt, newUserInput);
                var content = new StringContent(JsonConvert.SerializeObject(requestBody, Formatting.None, _serializerSettings),
                    Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseBody);
                var geminiResponseText = geminiResponse?.Candidates[0].Content.Parts[0].Text;

                _logger.LogInformation("Finish generating content from Gemini");
                return geminiResponseText;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when generating content from Gemini with error: {Message}",
                    e.InnerException?.Message);
                throw;
            }
        }
    }
}