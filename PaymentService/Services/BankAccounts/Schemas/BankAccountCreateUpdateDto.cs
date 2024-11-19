using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace PaymentService.Services.BankAccounts.Schemas
{
    public class BankAccountCreateUpdateDto : IValidatableObject
    {
        public int BankId { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string AccountName { get; set; }

        public bool IsPrimary { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BankId <= 0)
            {
                yield return new ValidationResult("BankId is greater than 0", new[] { nameof(BankId) });
            }
        }
    }
}