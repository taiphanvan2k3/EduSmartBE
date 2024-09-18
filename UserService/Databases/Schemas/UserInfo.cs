using System.ComponentModel.DataAnnotations;

namespace UserService.Databases.Schemas
{
    public class UserInfo
    {
        [MaxLength(100)]
        public string UserId { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public string AvatarURL { get; set; }

        public string Phone { get; set; }

        public int Gender { get; set; }
    }
}