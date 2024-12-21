using CourseManagementService.Settings;
using Microsoft.Extensions.Options;

namespace CourseManagementService.Middlewares
{
    /// <summary>
    /// Automatically injects the necessary headers for Gemini API requests.
    /// </summary>
    public class GeminiDelegatingHandler(IOptions<GeminiSetting> geminiSetting)
        : DelegatingHandler
    {
        private readonly GeminiSetting _geminiSetting = geminiSetting.Value;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("x-goog-api-key", $"{_geminiSetting.ApiKey}");

            return base.SendAsync(request, cancellationToken);
        }
    }
}