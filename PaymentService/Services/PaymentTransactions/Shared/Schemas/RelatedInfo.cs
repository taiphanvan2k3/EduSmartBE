namespace PaymentService.Services.PaymentTransactions.Shared.Schemas
{
    public static class RelatedInfo
    {
        public class BuyCourse
        {
            public Guid CourseId { get; set; }

            public int TeacherId { get; set; }
        }
    }
}