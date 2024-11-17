namespace PaymentService.Databases.Schemas
{
    public class BaseEntity
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}