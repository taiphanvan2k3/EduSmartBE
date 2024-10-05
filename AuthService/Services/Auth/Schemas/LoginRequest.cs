using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Auth.Schemas
{
    public class LoginRequest
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public LoginRequest()
        {
            Email = "";
            UserName = "";
        }
    }
}