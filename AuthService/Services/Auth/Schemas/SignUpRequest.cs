using System.ComponentModel.DataAnnotations;
using AuthService.Commons;
using AuthService.CustomAttributes;
using AuthService.Enumerations;

namespace AuthService.Services.Auth.Schemas
{
    public class SignUpRequest : IValidatableObject
    {
        [Required]
        public string Email { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        [Required]
        [ProviderValidation(ProviderType.Email, ProviderType.Google, ProviderType.Facebook, ProviderType.GitHub)]
        public string Provider { get; set; }

        /// <summary>
        /// This property is used when sign up with social providers.
        /// </summary>
        public string AvatarURL { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Provider == ProviderType.Email && string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Password is required when sign up with email.", [nameof(Password)]);
            }
        }
    }
}