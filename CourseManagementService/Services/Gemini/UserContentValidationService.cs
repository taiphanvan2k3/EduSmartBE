using CourseManagementService.Common;
using CourseManagementService.Services.Gemini.HttpClientModels;
using CourseManagementService.Services.Gemini.Prompts;
using CourseManagementService.Services.Gemini.Schemas;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CourseManagementService.Services.Gemini
{
    public interface IUserContentValidationService
    {
        public Task<ResponseInfo> ValidateUserContentAsync(string newUserInput);
    }

    public class UserContentValidationService(IServiceProvider serviceProvider,
        ILogger<UserContentValidationService> logger) : BaseService(serviceProvider, logger), IUserContentValidationService
    {
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        private readonly GeminiClient _geminiClient = serviceProvider.GetRequiredService<GeminiClient>();

        public async Task<ResponseInfo> ValidateUserContentAsync(string newUserInput)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                string geminiResponse = await _geminiClient.GenerateContentAsync(ContentValidationPrompt.Prompt, newUserInput);
                geminiResponse = geminiResponse.Replace("```json", "").Replace("```", "").Trim();
                var geminiResponseObject = JsonConvert.DeserializeObject<UserContentResponse>(geminiResponse, _serializerSettings);

                var responseInfo = new ResponseInfo();
                if (geminiResponseObject.Classification == ContentClassification.Valid)
                {
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                }
                else if (geminiResponseObject.Classification == ContentClassification.Invalid &&
                    (geminiResponseObject.ToxicLevel == ToxicityLevel.Medium || geminiResponseObject.ToxicLevel == ToxicityLevel.High))
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = "Content is not allowed due to inappropriate words.";
                }
                else if (geminiResponseObject.Classification == ContentClassification.Spam)
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = "Content is not allowed due to spam.";
                }

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}