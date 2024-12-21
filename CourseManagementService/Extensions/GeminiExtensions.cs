using CourseManagementService.Middlewares;
using CourseManagementService.Services.Gemini.HttpClientModels;
using CourseManagementService.Settings;
using Microsoft.Extensions.Options;

namespace CourseManagementService.Extensions
{
    public static class GeminiExtensions
    {
        public static void AddGemini(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GeminiSetting>(configuration.GetSection("GeminiSetting"));

            services.AddTransient<GeminiDelegatingHandler>();

            services.AddHttpClient<GeminiClient>(
                (serviceProvider, httpClient) =>
                {
                    var geminiOptions = serviceProvider.GetRequiredService<IOptions<GeminiSetting>>().Value;

                    httpClient.BaseAddress = new Uri(geminiOptions.Url);
                })
                .AddHttpMessageHandler<GeminiDelegatingHandler>();
        }
    }
}