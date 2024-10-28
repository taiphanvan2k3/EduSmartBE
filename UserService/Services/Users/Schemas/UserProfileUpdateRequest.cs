using System.ComponentModel.DataAnnotations;

namespace UserService.Services.Users.Schemas
{
    public class UserProfileUpdateRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Phone { get; set; }

        public int Gender { get; set; }
        
        public IFormFile File { get; set; }
    }
}