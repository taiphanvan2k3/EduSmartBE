using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.BackgroundServices;
using AuthService.Commons;
using AuthService.Services.Account.Schemas;
using AuthService.Services.Auth.Schemas;
using AuthService.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ApplicationUser = AuthService.Databases.Schemas.ApplicationUser;
using TblRefreshToken = AuthService.Databases.Schemas.RefreshToken;

namespace AuthService.Services.Auth
{
    public interface ITokenService
    {
        public AuthTokens GenerateTokens(UserInfo userInfo);

        public Task<ResponseInfo> DoRefreshToken(string refreshToken);

        public Task SaveRefreshToken(int userId, string refreshToken, string ipAddress);

        public Task<ResponseInfo> RenewRefreshToken(int userId, string oldRefreshToken, string newRefreshToken, string ipAddress);
    }

    public class TokenService(IServiceProvider serviceProvider,
        IOptions<JwtSetting> jwtSetting,
        UserManager<ApplicationUser> userManager,
        CommonProducer commonProducer,
        ILogger<TokenService> logger) : BaseService(serviceProvider, logger), ITokenService
    {
        private readonly JwtSetting _jwtSetting = jwtSetting?.Value
            ?? throw new ArgumentNullException(nameof(jwtSetting));

        private readonly UserManager<ApplicationUser> _userManager = userManager
            ?? throw new ArgumentNullException(nameof(userManager));

        private readonly CommonProducer _commonProducer = commonProducer
            ?? throw new ArgumentNullException(nameof(commonProducer));

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

        public async Task SaveRefreshToken(int userId, string refreshToken, string ipAddress)
        {
            try
            {
                _logger.LogInformation("[TokenService][SaveRefreshToken] Start");
                var refreshTokenEntity = new TblRefreshToken()
                {
                    Token = refreshToken,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.RefreshTokenExpirationInMinutes),
                    CreatedAt = DateTime.UtcNow
                };

                await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("[TokenService][SaveRefreshToken] End");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TokenService][SaveRefreshToken] Error");
                throw;
            }
        }

        public async Task<ResponseInfo> DoRefreshToken(string refreshToken)
        {
            try
            {
                _logger.LogInformation("[TokenService][DoRefreshToken] Start");
                var responseInfo = new ResponseInfo();

                var existedRefreshToken = await _context.RefreshTokens
                    .Where(x => x.Token == refreshToken && !x.IsRevoked && x.Expires > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (existedRefreshToken == null)
                {
                    return new ResponseInfo
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Refresh token is invalid"
                    };
                }

                var userInfo = await _context.Users
                    .Where(x => x.Id == existedRefreshToken.UserId)
                    .Select(x => new UserInfo
                    {
                        Id = x.Id,
                        Username = x.UserName,
                        Email = x.Email,
                    })
                    .FirstOrDefaultAsync();

                if (userInfo == null)
                {
                    return new ResponseInfo
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User not found"
                    };
                }

                userInfo.Roles = [.. (await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(
                    userInfo.Id.ToString())))];
                var tokens = GenerateTokens(userInfo);

                responseInfo.Data.Add("meta", new
                {
                    accessToken = tokens.AccessToken,
                    refreshToken = tokens.RefreshToken
                });
                responseInfo.Data.Add("AccessTokenExpireIn", _jwtSetting.TokenExpirationInMinutes);
                responseInfo.Data.Add("RefreshTokenExpireIn", _jwtSetting.RefreshTokenExpirationInMinutes);

                await _commonProducer.EnqueueDataAsync(new BackgroundJobData
                {
                    JobType = BackgroundJobType.RENEW_REFRESH_TOKEN,
                    Data = new Dictionary<string, dynamic>
                    {
                        {"oldRefreshToken", refreshToken},
                        {"newRefreshToken", tokens.RefreshToken},
                        { "userId", userInfo.Id },
                        { "ipAddress", _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString() ?? "::1" }
                    }
                });

                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TokenService][DoRefreshToken] Error");
                throw;
            }
        }

        public async Task<ResponseInfo> RenewRefreshToken(int userId, string oldRefreshToken, string newRefreshToken, string ipAddress)
        {
            try
            {
                _logger.LogInformation("[TokenService][RevokeRefreshToken] Start");
                var responseInfo = new ResponseInfo();
                var existedRefreshToken = await _context.RefreshTokens
                    .Where(x => x.Token == oldRefreshToken && x.UserId == userId)
                    .FirstOrDefaultAsync();

                if (existedRefreshToken == null)
                {
                    return new ResponseInfo
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Refresh token is invalid"
                    };
                }

                existedRefreshToken.IsRevoked = true;
                existedRefreshToken.RevokedAt = DateTime.UtcNow;

                await SaveRefreshToken(userId, newRefreshToken, ipAddress);

                _logger.LogInformation("[TokenService][RevokeRefreshToken] End");
                responseInfo.Message = "Revoke refresh token successfully";
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TokenService][RevokeRefreshToken] Error");
                throw;
            }
        }

        private string GenerateJwtToken(UserInfo userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Ở đây dùng mã ASCII cũng được vì secret key thường là tiếng Anh nên bảng mã ASCII có thể biểu diễn được
            var key = Encoding.ASCII.GetBytes(_jwtSetting.SecretKey);
            var claims = new List<Claim>
            {
                new("username", userInfo.Username),
                new(ClaimTypes.Email, userInfo.Email),
                new("userId", userInfo.Id.ToString()),
                new("iss", _jwtSetting.Issuer),
                new("aud", _jwtSetting.Audience)
            };

            claims.AddRange(userInfo.Roles.Select(role => new Claim("role", role)));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.TokenExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private RefreshTokenResponse GenerateRefreshToken()
        {
            return new RefreshTokenResponse
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.RefreshTokenExpirationInMinutes),
                Created = DateTime.UtcNow
            };
        }
    }
}