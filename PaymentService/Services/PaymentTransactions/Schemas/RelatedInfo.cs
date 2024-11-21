namespace PaymentService.Services.PaymentTransactions.Schemas
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