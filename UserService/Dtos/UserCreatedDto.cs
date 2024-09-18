using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class UserCreatedDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string CreateAt  { get; set; }
    }
}