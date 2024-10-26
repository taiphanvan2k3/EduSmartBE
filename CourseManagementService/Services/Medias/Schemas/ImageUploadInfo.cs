namespace CourseManagementService.Services.Medias.Schemas
{
    public class ImageUploadInfo(Guid courseId, string localImagePath)
    {
        public Guid CouseId { get; set; } = courseId;

        public string LocalImagePath { get; set; } = localImagePath;
    }
}