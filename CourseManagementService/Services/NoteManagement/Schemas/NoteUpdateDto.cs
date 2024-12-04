using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.NoteManagement.Schemas
{
    public class NoteUpdateDto
    {
        [SwaggerIgnore]
        public Guid Id { get; set; }

        public string Comment { get; set; }
    }
}