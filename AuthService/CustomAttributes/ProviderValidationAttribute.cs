using System.ComponentModel.DataAnnotations;

namespace AuthService.CustomAttributes
{
    public class ProviderValidationAttribute(params string[] validProviders) : ValidationAttribute
    {
        private readonly string[] _validProviders = validProviders;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string provider)
            {
                if (char.IsLower(provider[0]))
                {
                    return new ValidationResult("Provider must start with an uppercase letter.");
                }

                if (!_validProviders.Contains(provider))
                {
                    return new ValidationResult($"Provider must be one of the following: {string.Join(", ", _validProviders)}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}