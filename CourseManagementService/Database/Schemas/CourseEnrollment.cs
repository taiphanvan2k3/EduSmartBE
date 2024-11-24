using CourseManagementService.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class CourseEnrollment
    {
        public int StudentId { get; set; }

        public Guid CourseId { get; set; }

        public DateTimeOffset EnrollmentDate { get; set; }

        public DateTimeOffset? CompletionDate { get; set; }

        public DateTimeOffset? LeaveDate { get; set; }

        public bool IsCompleted { get; set; }

        [Comment("Allow other students to see the progress of this student in this course")]
        public CourseProgressVisibility VisibilityStatus { get; set; }

        public virtual Course Course { get; set; }
    }
}