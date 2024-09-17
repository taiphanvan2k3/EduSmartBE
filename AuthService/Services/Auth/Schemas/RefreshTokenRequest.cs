using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Auth.Schemas
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}