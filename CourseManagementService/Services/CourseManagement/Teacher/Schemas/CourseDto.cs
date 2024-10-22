using CourseManagementService.Common;

namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class CourseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string BriefDescription { get; set; }

        public string DetailedDescription { get; set; }

        public string ThumbnailURL { get; set; }

        public decimal Price { get; set; }

        public string CurrencyCode { get; set; }

        public int TotalStudents { get; set; }

        public int TotalLessons { get; set; }

        public int TotalMinutes { get; set; }

        public LookupDto Type { get; set; }

        public LookupDto Category { get; set; }

        public List<LookupDto> Tags { get; set; }
    }
}