using PaymentService.Enumerations;
 
namespace PaymentService.Services.TeacherEarnings.Schemas
{
    public class RevenueCourse
    {
        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }

        public string CourseName { get; set; }

        public RelatedInfo RelatedInfo { get; set; }
    }
}