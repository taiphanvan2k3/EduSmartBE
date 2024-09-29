using System.ComponentModel.DataAnnotations;
using AuthService.Enumerations;

namespace AuthService.Services.Auth.Schemas
{
    public class SignUpRequest
    {
        [Required]
        public string Email { get; set; }

        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}