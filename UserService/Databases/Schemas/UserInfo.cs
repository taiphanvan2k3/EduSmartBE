using System.ComponentModel.DataAnnotations;

namespace UserService.Databases.Schemas
{
    public class UserInfo
    {
        public int UserId { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public string AvatarURL { get; set; }

        public string Phone { get; set; }

        public int Gender { get; set; }

        public virtual User User { get; set; }
    }
}