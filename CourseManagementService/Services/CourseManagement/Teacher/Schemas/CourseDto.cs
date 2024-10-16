using CourseManagementService.Enumerations;

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

        public CourseType Type { get; set; }

        public int TeacherId { get; set; }

        public int CategoryId { get; set; }

        public List<int> TagIds { get; set; }
    }
}