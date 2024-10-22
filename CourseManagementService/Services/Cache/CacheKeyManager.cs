namespace CourseManagementService.Services.Cache
{
    public static class CacheManager
    {
        public static class PopularCourses
        {
            public static string Key => "PopularCourses";
            public static int ExpireTimeInMinutes => 10;
        }

        public static class RecommendedCourses
        {
            public static string Key(int userId) => $"RecommendedCourses_{userId}";
            public static int ExpireTimeInMinutes => 30;
        }

        public static class CourseDetails
        {
            public static string Key(int courseId) => $"CourseDetails_{courseId}";
            public static int ExpireTimeInMinutes => 60;
        }

        public static class CourseSearch
        {
            public static string Key(string keyword, int currentPage, int pageSize) => $"CourseSearch_{keyword}_{currentPage}_{pageSize}";
            public static int ExpireTimeInMinutes => 10;
        }
    }
}