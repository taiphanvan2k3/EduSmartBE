using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CourseManagementService.Common.Schemas
{
    public class UserDetail
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string AvatarURL { get; set; }

        [JsonPropertyName("roleInSystem")]
        [JsonProperty("roleInSystem")]
        public string Roles { get; set; }

        public string RoleInCourse { get; set; }
    }
}