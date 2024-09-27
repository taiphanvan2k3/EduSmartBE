using System.Net.Http.Headers;
using AuthService.Services.Auth.Schemas;
using AuthService.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Services.Auth
{
    public interface IGoogleAuthService
    {
        /// <summary>
        /// Get access token from AuthCode. This method will call Google API to get access token from AuthCode
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 17/09/2024</para>
        /// </summary>
        /// <param name="code">AuthCode from Google</param>
        /// <returns></returns>
        public Task<GoogleTokenResponse> GetAccessToken(string code);

        /// <summary>
        /// Get user info from access token. This method will call Google API to get user info from access token
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 17/09/2024</para>
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public Task<GoogleUserInfo> GetUserInfo(string accessToken);
    }

    public class GoogleAuthService(IServiceProvider serviceProvider,
        HttpClient httpClient,
        IOptions<GoogleAuthenticationSetting> googleAuthenticationSetting,
        ILogger<GoogleAuthService> logger) : BaseService(serviceProvider, logger), IGoogleAuthService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly GoogleAuthenticationSetting _googleAuthenticationSetting = googleAuthenticationSetting?.Value
            ?? throw new ArgumentNullException(nameof(googleAuthenticationSetting));

        public async Task<GoogleTokenResponse> GetAccessToken(string code)
        {
            var methodName = GetActualAsyncMethodName();
            _logger.LogInformation("[GoogleAuthService][{MethodName}] Start", methodName);

            var tokenRequestBody = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", _googleAuthenticationSetting.ClientId},
                {"client_secret", _googleAuthenticationSetting.ClientSecret},
                {"redirect_uri", _googleAuthenticationSetting.RedirectUrl},
                {"grant_type", "authorization_code"},
            };

            try
            {
                var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(tokenRequestBody));
                tokenResponse.EnsureSuccessStatusCode();

                var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
                var googleTokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(tokenResponseContent);

                _logger.LogInformation("[GoogleAuthService][{MethodName}] End", methodName);
                return googleTokenResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GoogleAuthService][{MethodName}] Error", methodName);
                throw;
            }
        }

        public async Task<GoogleUserInfo> GetUserInfo(string accessToken)
        {
            var methodName = GetActualAsyncMethodName();
            _logger.LogInformation("[GoogleAuthService][{MethodName}] Start", methodName);
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var userInfoResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v1/userinfo");
                userInfoResponse.EnsureSuccessStatusCode();

                var userInfoResponseContent = await userInfoResponse.Content.ReadAsStringAsync();
                var googleUserInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResponseContent);

                _logger.LogInformation("[GoogleAuthService][{MethodName}] End", methodName);
                return googleUserInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GoogleAuthService][{MethodName}] Error", methodName);
                throw;
            }
        }
    }
}