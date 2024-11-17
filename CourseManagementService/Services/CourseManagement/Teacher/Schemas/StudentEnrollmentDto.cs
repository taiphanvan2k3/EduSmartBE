using Newtonsoft.Json;

namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class StudentEnrollmentDto
    {
        public int StudentId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string AvatarURL { get; set; }
    }

    public class StudentEnrollmentOverall : StudentEnrollmentDto
    {
        [JsonProperty(Order = 6)]
        public int EnrollmentCount { get; set; }
    }

    public class StudentEnrollmentInCourse : StudentEnrollmentDto
    {
        [JsonProperty(Order = 6)]
        public DateTimeOffset EnrollmentDate { get; set; }

        [JsonProperty(Order = 7)]
        public DateTimeOffset? LeaveDate { get; set; }

        [JsonProperty(Order = 8)]
        public bool IsActive { get; set; } = true;
    }
}