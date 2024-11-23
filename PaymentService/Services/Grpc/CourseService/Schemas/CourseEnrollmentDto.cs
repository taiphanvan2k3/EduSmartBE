namespace PaymentService.Services.Grpc.CourseService.Schemas
{
    public class CourseEnrollmentDto
    {
        public string CourseId { get; set; }

        public int StudentId { get; set; }

        public DateTimeOffset EnrollmentDate { get; set; }
    }
}