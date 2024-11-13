using System.ComponentModel;

namespace PaymentService.Databases.Schemas
{
    public class BaseEntity
    {
        [DefaultValue("CURRENT_TIMESTAMP")]
        public DateTimeOffset CreatedAt { get; set; }

        [DefaultValue("CURRENT_TIMESTAMP")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}