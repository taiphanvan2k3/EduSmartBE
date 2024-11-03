namespace CourseManagementService.Services.Medias.Schemas
{
    public class ImageUploadInfo(Guid resourceId, string localImagePath)
    {
        public Guid ResourceId { get; set; } = resourceId;

        public string LocalImagePath { get; set; } = localImagePath;
    }
}