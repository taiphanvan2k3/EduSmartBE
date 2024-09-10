using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Permission.Schemas.Screen
{
    public class ScreenUpdateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public int Order { get; set; }
    }
}