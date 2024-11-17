using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Databases.Schemas
{
    public class Bank : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShortName { get; set; }

        // Example: 970415 -> VietinBank
        [Required]
        [MaxLength(10)]
        [Comment("Bank Identification Number")]
        public string Bin { get; set; }

        [MaxLength(255)]
        public string LogoURL { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; } = [];
    }
}