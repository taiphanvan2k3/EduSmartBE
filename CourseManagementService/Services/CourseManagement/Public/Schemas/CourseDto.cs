using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.CategoryManagement.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
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

        [JsonIgnore]
        public List<long> TotalSecondsByChapter { get; set; } = [];

        public string Duration
        {
            get
            {
                var totalSeconds = TotalSecondsByChapter.Sum();
                return Utils.ConvertSecondsToDuration(totalSeconds);
            }
        }

        public long DurationInSeconds
        {
            get
            {
                return TotalSecondsByChapter.Sum();
            }
        }

        public int TotalLessons { get; set; }

        public bool IsRegistered { get; set; }

        public LookupDto Type { get; set; }

        public CategoryDto Category { get; set; }

        public List<LookupDto> Tags { get; set; }
    }
}