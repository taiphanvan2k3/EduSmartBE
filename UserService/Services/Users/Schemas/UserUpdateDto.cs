using System.ComponentModel.DataAnnotations;

namespace UserService.Services.Users.Schemas
{
    public class UserUpdateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Phone { get; set; }

        public int Gender { get; set; }
    }
}