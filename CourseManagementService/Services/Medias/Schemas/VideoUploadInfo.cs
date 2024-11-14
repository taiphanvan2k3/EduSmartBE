namespace CourseManagementService.Services.Medias.Schemas
{
    public class VideoUploadInfo(Guid resourceId, string localPath)
    {
        public Guid ResourceId { get; set; } = resourceId;

        public string LocalPath { get; set; } = localPath;
    }
}