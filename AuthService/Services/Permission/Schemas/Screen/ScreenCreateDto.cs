using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Permission.Schemas.Screen
{
    public class ScreenCreateDto : ScreenUpdateDto
    {
        [Required]
        public string Id { get; set; }
    }
}