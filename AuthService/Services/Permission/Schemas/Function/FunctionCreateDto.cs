using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Permission.Schemas.Function
{
    public class FunctionCreateDto : FunctionUpdateDto
    {
        [Required]
        public string Id { get; set; }
    }
}