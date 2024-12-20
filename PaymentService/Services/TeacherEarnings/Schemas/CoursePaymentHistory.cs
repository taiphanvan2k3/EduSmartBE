using PaymentService.Commons.Schemas;

namespace PaymentService.Services.TeacherEarnings.Schemas
{
    public class CoursePaymentHistory
    {
        public string PaymentCode { get; set; }

        public UserDetail StudentInfo { get; set; }

        public CourseDetail CourseInfo { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTimeOffset EnrollmentDate { get; set; }
    }
}