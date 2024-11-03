namespace CourseManagementService.Services.Medias.Schemas
{
    public class VideoUploadInfo(Guid lessonId, string localPath)
    {
        public Guid LessonId { get; set; } = lessonId;

        public string LocalPath { get; set; } = localPath;
    }
}