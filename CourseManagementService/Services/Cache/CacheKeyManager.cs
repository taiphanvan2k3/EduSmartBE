using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.CourseManagement.Public.Schemas;

namespace CourseManagementService.Services.Cache
{
    public static class CacheManager
    {
        public static class User
        {
            public static string Key(int userId, bool withRole = false)
            {
                return withRole ? $"User_WithRole:{userId}" : $"User:{userId}";
            }

            public static int ExpireTimeInMinutes => 60 * 24 * 7;
        }

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

        /// <summary>
        /// Danh sách khoá học mà giảng viên đang sở hữu
        /// </summary>
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

        /// <summary>
        /// Giảng viên xem danh sách học viên đã đăng ký (tổng thể hoặc trong một khoá học)
        /// </summary>
        public static class StudentEnrollments
        {
            public static class Overall
            {
                public static string Key(int userId, int currentPage, int pageSize) => $"StudentEnrollments_Overall_{userId}_{currentPage}_{pageSize}";
                public static int ExpireTimeInMinutes => 10;
            }

            public static class InCourse
            {
                public static string Key(Guid courseId, int currentPage, int pageSize) => $"StudentEnrollments_InCourse_{courseId}_{currentPage}_{pageSize}";
                public static int ExpireTimeInMinutes => 10;
            }
        }

        public static class CourseSearch
        {
            public static string Key(string keyword, int currentPage, int pageSize) => $"CourseSearch_{keyword}_{currentPage}_{pageSize}";
            public static int ExpireTimeInMinutes => 10;
        }

        public static class CourseSearchByCategory
        {
            public static string Key(int categoryId, int userId, PublicCourseSearchWithoutCategoryCondition singleSort)
            {
                var tailPart = $"{userId}_{singleSort.Keyword}_{singleSort.CurrentPage}_{singleSort.PageSize}_{singleSort.SortBy}_{singleSort.SortDirection}";
                return $"CourseSearchByCategory_{categoryId}_{Utils.ConvertStringToBase64(tailPart)}";
            }
            public static int ExpireTimeInMinutes => 5;
        }

        public static class CourseDetail
        {
            public static string PrefixKey => "CourseDetail";
            public static string GetPrefixKey(Guid courseId) => $"{PrefixKey}_{courseId}";
            public static string Key(Guid courseId, int userId) => $"CourseDetail_{courseId}_{userId}";
            public static int ExpireTimeInMinutes => 30;
        }

        /// <summary>
        /// Người dùng khác xem tiến độ học tập của 1 học viên khác
        /// </summary>
        public static class CourseProgressOfOtherUser
        {
            public static string Key(int userId) => $"CourseProgressOfOtherUser_{userId}";
            public static int ExpireTimeInMinutes => 15;
        }

        /// <summary>
        /// Danh sách khoá học mà học viên đã đăng ký
        /// </summary>
        public static class EnrolledCourses
        {
            public static string Key(int userId) => $"EnrolledCourses_{userId}";
            public static int ExpireTimeInMinutes => 15;
        }

        public static class TeacherIdOfCourse
        {
            public static string KeyFromCourseId(Guid courseId) => $"TeacherIdOfCourse:FromCourseId:{courseId}";
            public static string KeyFromDiscussionId(Guid discussionId) => $"TeacherIdOfDiscussion:FromDiscussionId:{discussionId}";
            public static int ExpireTimeInMinutes => 60 * 24 * 100; // 100 days
        }

        public static class DiscussionInCourse
        {
            public static string Key(Guid courseId, int userId, string keyword, int currentPage, int pageSize)
            {
                return $"DiscussionInCourse:{courseId}:{userId}_{keyword}_{currentPage}_{pageSize}";
            }

            public static int ExpireTimeInMinutesForStudent => 60;
            public static int ExpireTimeInMinutesForTeacher => 10;
        }

        public static class Bookmark
        {
            public static string Key(Guid courseId, int userId) => $"BookmarkedLesson:{courseId}:{userId}";
            public static int ExpireTimeInMinutes => 60 * 24 * 7;
        }
    }
}