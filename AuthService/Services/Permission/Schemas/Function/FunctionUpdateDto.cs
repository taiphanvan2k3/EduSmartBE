using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Permission.Schemas.Function
{
    public class FunctionUpdateDto
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ScreenId { get; set; }

        [Required]
        public required int Order { get; set; }
    }
}
