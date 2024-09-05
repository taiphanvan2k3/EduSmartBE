using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Services.Auth.Schemas;
using AuthService.Services.User.Schemas;
using AuthService.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Auth
{
    public interface ITokenService
    {
        public AuthTokens GenerateTokens(UserInfo userInfo);
    }

    public class TokenService(IServiceProvider serviceProvider,
        IOptions<JwtSetting> jwtSetting,
        ILogger<TokenService> logger) : BaseService(serviceProvider, logger), ITokenService
    {
        private readonly JwtSetting _jwtSetting = jwtSetting?.Value
            ?? throw new ArgumentNullException(nameof(jwtSetting));

        public AuthTokens GenerateTokens(UserInfo userInfo)
        {
            var jwtToken = GenerateJwtToken(userInfo);
            var refreshToken = GenerateRefreshToken();

            return new AuthTokens
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpireIn = _jwtSetting.TokenExpirationInMinutes
            };
        }

        private string GenerateJwtToken(UserInfo userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Ở đây dùng mã ASCII cũng được vì secret key thường là tiếng Anh nên bảng mã ASCII có thể biểu diễn được
            var key = Encoding.ASCII.GetBytes(_jwtSetting.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity([
                    new Claim("username", userInfo.UserName),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim("userId", userInfo.Id.ToString()),
                    new Claim("roles", string.Join(",", userInfo.Roles)),
                    new Claim("iss", _jwtSetting.Issuer),
                    new Claim("aud", _jwtSetting.Audience),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.TokenExpirationInMinutes),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.RefreshTokenExpirationInDays),
                Created = DateTime.UtcNow
            };
        }
    }
}