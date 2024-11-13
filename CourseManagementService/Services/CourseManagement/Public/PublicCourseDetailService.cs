using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.AppState.Schemas;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CategoryManagement.Schemas;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc;
using CourseManagementService.Services.LessonManagement.LessonBase;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Public
{
    public interface IPublicCourseDetailService
    {
        /// <summary>
        /// Get course detail by course ID
        /// <para>Created at: 2024/11/06</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Task<CourseDetail> GetCourseDetail(Guid courseId);
    }

    public class PublicCourseDetailService(IServiceProvider serviceProvider,
        ILogger<ListOfPublicCourseService> logger)
        : BaseService(serviceProvider, logger), IPublicCourseDetailService
    {
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IGrpcUserService"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("ILessonBaseDetailService"));
        private readonly IListOfChaptersService _chapterService = serviceProvider.GetService<IListOfChaptersService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IListOfChaptersService"));

        public async Task<CourseDetail> GetCourseDetail(Guid courseId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                UserInfoState currentUser = GetCurrentUser();
                string cacheKey;
                if (currentUser == null || !await _lessonBaseDetailService.CanAccessCourseMaterial(courseId, currentUser.UserId))
                {
                    cacheKey = CacheManager.CourseDetail.Key(courseId, 0);
                }
                else
                {
                    cacheKey = CacheManager.CourseDetail.Key(courseId, currentUser.UserId);
                }

                var courseDetail = _cacheService.GetData<CourseDetail>(cacheKey);
                if (courseDetail != null)
                {
                    if (courseDetail.Course.IsRegistered)
                    {
                        courseDetail.LearnedLessons = await _lessonBaseDetailService.GetLearnedLessons(courseId, currentUser.UserId);
                    }
                    return courseDetail;
                }

                courseDetail = await _context.Courses
                    .Where(x => x.Id == courseId)
                    .Select(x => new CourseDetail()
                    {
                        Teacher = new TeacherDetail()
                        {
                            Id = x.TeacherId
                        },
                        Course = new CourseDto()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            CoreValues = x.CoreValues,
                            Prerequisites = x.Prerequisites,
                            Price = x.Price,
                            CurrencyCode = x.Currency.Code,
                            Type = new LookupDto()
                            {
                                Id = EnumHelper.ConvertEnumToInt(x.Type).ToString(),
                                Name = x.Type.ToString()
                            },
                            Category = new CategoryDto()
                            {
                                Id = x.Category.Id,
                                Name = x.Category.Name,
                                WebIconInfo = new IconInfoDto()
                                {
                                    Icon = x.Category.WebIconInfo.Icon,
                                    Color = x.Category.WebIconInfo.Color
                                },
                                MobileIconInfo = new IconInfoDto()
                                {
                                    Icon = x.Category.MobileIconInfo.Icon,
                                    Color = x.Category.MobileIconInfo.Color
                                }
                            },
                            Tags = x.Tags.Select(x => new LookupDto()
                            {
                                Id = x.Tag.Id.ToString(),
                                Name = x.Tag.Name
                            })
                            .ToList(),
                            ThumbnailURL = x.ThumbnailURL,
                            TotalStudents = x.Enrollments.Count,
                            TotalLessons = x.Chapters.SelectMany(x => x.Lessons).Count(),
                            TotalSecondsByChapter = x.Chapters.Select(c => c.Lessons.Sum(l => l.DurationInSeconds)).ToList(),
                            IsRegistered = currentUser != null && x.Enrollments.Any(x => x.StudentId == currentUser.UserId)
                        }
                    })
                    .FirstOrDefaultAsync();

                if (courseDetail == null)
                {
                    return null;
                }

                await FillTeacherInfo(courseDetail);
                courseDetail.Chapters = await _chapterService.GetListOfChaptersByCourseId(courseId);

                if (courseDetail.Course.IsRegistered)
                {
                    courseDetail.LearnedLessons = await _lessonBaseDetailService.GetLearnedLessons(courseId, currentUser.UserId);
                }

                _cacheService.SetData(cacheKey, courseDetail, DateTimeOffset.Now.AddMinutes(CacheManager.CourseDetail.ExpireTimeInMinutes));

                LogInfo("End", method);
                return courseDetail;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        private async Task FillTeacherInfo<T>(T course) where T : ICourseWithTeacher
        {
            var teachers = await _grpcUserService.GetListOfTeachers([course.Teacher.Id]);
            var teacher = teachers.Users.FirstOrDefault();

            if (teacher != null)
            {
                course.Teacher.FullName = teacher.FullName;
                course.Teacher.AvatarURL = teacher.AvatarURL;
                course.Teacher.Email = teacher.Email;
            }
        }
    }
}