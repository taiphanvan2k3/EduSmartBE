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

            // Khi inject GeminiClient vào các service khác, nó sẽ sử dụng cấu hình này, handler này thiết lập cho HttpClient
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