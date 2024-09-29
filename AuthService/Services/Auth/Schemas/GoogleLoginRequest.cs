using System.ComponentModel.DataAnnotations;
using AuthService.Enumerations;

namespace AuthService.Services.Auth.Schemas
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }

        public string Code { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}