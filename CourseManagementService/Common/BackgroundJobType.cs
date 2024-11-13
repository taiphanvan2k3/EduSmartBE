namespace CourseManagementService.Common
{
    public static class BackgroundJobType
    {
        public const string UpdateCourseThumbnail = "UpdateCourseThumbnail";
        public const string UpdateCoursePreviewVideo = "UpdateCoursePreviewVideo";
        public const string DeleteCourseThumbnail = "DeleteCourseThumbnail";
        public const string UpdateLessonVideo = "UpdateVideoLesson";
        public const string UpdateLessonThumbnail = "UpdateLessonThumbnail";
    }
}