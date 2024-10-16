namespace CourseManagementService.Database.Schemas
{
    public class CourseEnrollment
    {
        public int StudentId { get; set; }

        public Guid CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public virtual Course Course { get; set; }
    }
}