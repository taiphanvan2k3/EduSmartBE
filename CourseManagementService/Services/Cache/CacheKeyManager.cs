using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.CourseManagement.Public.Schemas;

namespace CourseManagementService.Services.Cache
{
    public static class CacheManager
    {
        public static class PopularCourses
        {
            public static string Key(int userId) => $"PopularCourses_{userId}";
            public static int ExpireTimeInMinutes => 10;
        }

        public static class RecommendedCourses
        {
            public static string Key(int userId) => $"RecommendedCourses_{userId}";
            public static int ExpireTimeInMinutes => 30;
        }

        public static class OwnedCourses
        {
            public static string Key(int userId) => $"OwnedCourses_{userId}";
            public static int ExpireTimeInMinutes => 12 * 60;
        }

        public static class TeachingAnalysis
        {
            public static string Key(int userId) => $"TeachingAnalysis_{userId}";
            public static int ExpireTimeInMinutes => 10;
        }

        public static class CourseSearch
        {
            public static string Key(string keyword, int currentPage, int pageSize) => $"CourseSearch_{keyword}_{currentPage}_{pageSize}";
            public static int ExpireTimeInMinutes => 10;
        }

        public static class CourseSearchByCategory
        {
            public static string Key(int categoryId, int userId, PublicCourseSearchCondition singleSort)
            {
                var tailPart = $"{userId}_{singleSort.Keyword}_{singleSort.CurrentPage}_{singleSort.PageSize}_{singleSort.SortBy}_{singleSort.SortDirection}";
                return $"CourseSearchByCategory_{categoryId}_{Utils.ConvertStringToBase64(tailPart)}";
            }
            public static int ExpireTimeInMinutes => 5;
        }

        public static class CourseDetail
        {
            public static string PrefixKey => "CourseDetail";
            public static string Key(Guid courseId, int userId) => $"CourseDetail_{courseId}_{userId}";
            public static int ExpireTimeInMinutes => 30;
        }
    }
}