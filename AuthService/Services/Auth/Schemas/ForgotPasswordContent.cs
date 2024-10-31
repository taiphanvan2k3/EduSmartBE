using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Auth.Schemas
{
    public class ForgotPasswordContent
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
    }
}