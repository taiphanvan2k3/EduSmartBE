namespace CourseManagementService.Common
{
    public static class Constants
    {
        public static readonly string ADMIN_EMAIL = "teampblpro@gmail.com";

        public static readonly string DEFAULT_ADMIN_PASSWORD = "Admin@123";

        public static readonly string SERVICE_NAME = (typeof(Constants).Namespace ?? "CourseManagementService").Split('.')[0];

        public static readonly string CONNECTION_STRING = "Host=edusmart.info.vn;Port=5432;Database=EduSmart.CourseManagementService;Username=pbl6duter;Password=Edusmartk21;Trust Server Certificate=true";

        public static readonly string UPLOAD_FOLDER_NAME = "Uploads";

        public static readonly string CLOUDINARY_URL_PREFIX = "https://res.cloudinary.com/da1aqhx1g/image/upload/";

        public static readonly string AZURE_STORAGE_URL_PREFIX = "https://edusmart.blob.core.windows.net/";

        public static readonly string DEFAULT_COURSE_THUMBNAIL = "https://res.cloudinary.com/da1aqhx1g/image/upload/f_auto,q_auto/v1/default-assets/x9tju6lpmexbed2k9rlz";

        public static readonly string IN_PROGRESS_THUMBNAIL = "https://res.cloudinary.com/da1aqhx1g/image/upload/f_auto,q_auto/v1/default-assets/rklr1cd3da0mkulzjq6b";

        public static class Role
        {
            public const string TEACHER = "Teacher";

            public const string STUDENT = "Student";
        }
    }
}