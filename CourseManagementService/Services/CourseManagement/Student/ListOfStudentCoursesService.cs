using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Student.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc.UserService;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IListOfStudentCoursesService
    {
        /// <summary>
        /// Get all courses that student enrolled
        /// <para>Created at: 2024/10/19</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> GetEnrolledCourses();

        /// <summary>
        /// View course progress of one student
        /// <para>Created at: 2024/11/23</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="userId">Id of student to view</param>
        /// <param name="isGetAll">True to get all courses, false to get only public courses</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetCoursesByUserId(int userId, bool isGetAll = false);


        /// <summary>
        /// Update the visibility status of all courses (private/friends/public)
        /// <para>Created at: 2024/11/23</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateAllCourseVisibilityStatus(UpdateAllVisibilityStatus request);
    }

    public class ListOfStudentCoursesService(IServiceProvider serviceProvider, ILogger<ListOfStudentCoursesService> logger)
        : BaseService(serviceProvider, logger), IListOfStudentCoursesService
    {
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IGrpcUserService"));
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("ICacheService"));

        public async Task<ResponseInfo> GetEnrolledCourses()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var currentUser = GetCurrentUser();
                var responseInfo = await GetCoursesByUserId(currentUser.UserId, isGetAll: true);

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> GetCoursesByUserId(int userId, bool isGetAll = false)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                if (!await _grpcUserService.CheckUserExist(userId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Student not found");
                }

                var currentUser = GetCurrentUser();
                isGetAll = isGetAll || currentUser.UserId == userId;

                var cacheKey = isGetAll
                    ? CacheManager.EnrolledCourses.Key(userId)
                    : CacheManager.CourseProgressOfOtherUser.Key(userId);

                var cachedData = _cacheService.GetData<List<EnrolledCourseInfo>>(cacheKey);

                if (cachedData != null)
                {
                    responseInfo.Data.Add("courses", cachedData);
                    return responseInfo;
                }

                var visibilityEnumType = typeof(CourseProgressVisibility);
                var courses = await _context.CourseEnrollments
                    .Where(x => x.StudentId == userId && (isGetAll || x.VisibilityStatus == CourseProgressVisibility.Public))
                    .Select(x => new EnrolledCourseInfo()
                    {
                        Id = x.CourseId,
                        Name = x.Course.Name,
                        ThumbnailURL = x.Course.ThumbnailURL,
                        Description = x.Course.Description,
                        TotalStudents = x.Course.Enrollments.Count,
                        VisibilityStatus = new LookupDto()
                        {
                            Id = ((int)Enum.Parse(visibilityEnumType, x.VisibilityStatus.ToString())).ToString(),
                            Name = Utils.GetEnumName(x.VisibilityStatus)
                        },
                        TotalLessons = x.Course.Chapters.Sum(ch => ch.Lessons.Count),
                        Teacher = new TeacherDetail()
                        {
                            Id = x.Course.TeacherId
                        }
                    })
                    .ToListAsync();

                if (courses.Count > 0)
                {
                    await FillTeachersInfo(courses);

                    var courseIds = courses.Select(x => x.Id).ToList();
                    var courseIdsDict = courses.ToDictionary(x => x.Id, x => x);

                    var lessonTrackings = await _context.LessonTrackings
                        .Where(x => x.StudentId == userId && courseIds.Contains(x.CourseId))
                        .GroupBy(x => x.CourseId)
                        .Select(x => new
                        {
                            CourseId = x.Key,
                            CompletedLessonCount = x.Count(l => l.IsCompleted),
                            TimeSpent = x.Sum(x => x.TimeSpent)
                        })
                        .ToListAsync();

                    foreach (var lessonTracking in lessonTrackings)
                    {
                        if (courseIdsDict.TryGetValue(lessonTracking.CourseId, out var course))
                        {
                            course.CompletedLessons = lessonTracking.CompletedLessonCount;
                            course.TimeSpent = lessonTracking.TimeSpent;
                        }
                    }
                }

                _cacheService.SetData(cacheKey, courses, DateTimeOffset.Now.AddMinutes(
                    CacheManager.CourseProgressOfOtherUser.ExpireTimeInMinutes));

                LogInfo("End", method);
                responseInfo.Data.Add("courses", courses);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateAllCourseVisibilityStatus(UpdateAllVisibilityStatus request)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var currentUser = GetCurrentUser();
                await _context.CourseEnrollments
                    .Where(c => c.StudentId == currentUser.UserId)
                    .ExecuteUpdateAsync(c => c.SetProperty(x => x.VisibilityStatus, request.VisibilityStatus));

                _cacheService.RemoveData(CacheManager.CourseProgressOfOtherUser.Key(currentUser.UserId));
                _cacheService.RemoveData(CacheManager.EnrolledCourses.Key(currentUser.UserId));

                responseInfo.Data.Add("currentStatus", Utils.GetEnumName(request.VisibilityStatus));

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        private async Task FillTeachersInfo(List<EnrolledCourseInfo> courses)
        {
            var teacherIds = courses.Select(x => x.Teacher.Id).Distinct().ToList();

            var teachersResponse = await _grpcUserService.GetListOfTeachers(teacherIds);
            Dictionary<int, GrpcServices.SimpleUserResponse> teachersDict = teachersResponse.Users
                .ToDictionary(x => x.Id, x => x);

            foreach (var course in courses)
            {
                if (teachersDict.TryGetValue(course.Teacher.Id, out var teacherInfo))
                {
                    course.Teacher.FullName = teacherInfo.FullName;
                    course.Teacher.Email = teacherInfo.Email;
                    course.Teacher.AvatarURL = teacherInfo.AvatarURL;
                }
            }
        }
    }
}