using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.AppState.Schemas;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Public
{
    public interface IListOfPublicCourseService
    {
        public Task<PaginatedList<CourseDetailWithTeacherDto>> GetPopularCourses();
        public Task<PaginatedList<CourseDetailWithTeacherDto>> GetRecommendedCourses();
    }

    public class ListOfPublicCourseService(IServiceProvider serviceProvider,
        ILogger<ListOfPublicCourseService> logger)
        : BaseService(serviceProvider, logger), IListOfPublicCourseService
    {
        private readonly int _maxRecommendedCourses = 10;
        private readonly int _maxPopularCourses = 10;

        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IGrpcUserService"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));

        public async Task<PaginatedList<CourseDetailWithTeacherDto>> GetPopularCourses()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var cacheKey = CacheManager.PopularCourses.Key;
                var cacheValue = _cacheService.GetData<PaginatedList<CourseDetailWithTeacherDto>>(cacheKey);

                if (cacheValue != null)
                {
                    LogInfo("End", method);
                    return cacheValue;
                }

                (List<Guid> enrolledCourseIds, List<int> enrolledCourseCategories) = await GetEnrolledCoursesAndCategories();
                var courses = await _context.Courses
                    .Where(x => !enrolledCourseIds.Contains(x.Id))
                    .OrderByDescending(x => x.Enrollments.Count)
                    .ThenByDescending(x => enrolledCourseCategories.Contains(x.CategoryId)) // Sắp xếp theo category đã tham gia
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(c => new CourseDetailWithTeacherDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        BriefDescription = c.BriefDescription,
                        DetailedDescription = c.DetailedDescription,
                        Price = c.Price,
                        CurrencyCode = c.Currency.Code,
                        Type = new LookupDto()
                        {
                            Id = EnumHelper.ConvertEnumToInt(c.Type).ToString(),
                            Name = c.Type.ToString()
                        },
                        Category = new LookupDto()
                        {
                            Id = c.Category.Id.ToString(),
                            Name = c.Category.Name
                        },
                        Tags = c.Tags.Select(x => new LookupDto()
                        {
                            Id = x.Tag.Id.ToString(),
                            Name = x.Tag.Name
                        })
                        .ToList(),
                        ThumbnailURL = c.ThumbnailURL,
                        TotalStudents = c.Enrollments.Count,
                        Teacher = new TeacherDetail()
                        {
                            Id = c.TeacherId
                        }
                    })
                    .ToPaginatedListAsync(currentPage: 1, _maxPopularCourses);

                await FillTeacherInfo(courses.Items);

                _cacheService.SetData(cacheKey, courses, DateTimeOffset.Now.AddMinutes(CacheManager.PopularCourses.ExpireTimeInMinutes));

                LogInfo("End", method);
                return courses;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<PaginatedList<CourseDetailWithTeacherDto>> GetRecommendedCourses()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                UserInfoState currentUser = GetCurrentUser();

                var cacheKey = CacheManager.RecommendedCourses.Key(currentUser?.UserId ?? 0);
                var cacheValue = _cacheService.GetData<PaginatedList<CourseDetailWithTeacherDto>>(cacheKey);
                if (cacheValue != null)
                {
                    LogInfo("End", method);
                    return cacheValue;
                }

                (List<Guid> enrolledCourseIds, List<int> enrolledCourseCategories) = await GetEnrolledCoursesAndCategories();
                var courses = await _context.Courses
                    .Where(x => !enrolledCourseIds.Contains(x.Id))
                    .OrderByDescending(x => enrolledCourseCategories.Contains(x.CategoryId)) // Sắp xếp theo category đã tham gia
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(c => new CourseDetailWithTeacherDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        BriefDescription = c.BriefDescription,
                        DetailedDescription = c.DetailedDescription,
                        Price = c.Price,
                        CurrencyCode = c.Currency.Code,
                        Type = new LookupDto()
                        {
                            Id = EnumHelper.ConvertEnumToInt(c.Type).ToString(),
                            Name = c.Type.ToString()
                        },
                        Category = new LookupDto()
                        {
                            Id = c.Category.Id.ToString(),
                            Name = c.Category.Name
                        },
                        Tags = c.Tags.Select(x => new LookupDto()
                        {
                            Id = x.Tag.Id.ToString(),
                            Name = x.Tag.Name
                        })
                        .ToList(),
                        ThumbnailURL = c.ThumbnailURL,
                        TotalStudents = c.Enrollments.Count,
                        Teacher = new TeacherDetail()
                        {
                            Id = c.TeacherId
                        }
                    })
                    .ToPaginatedListAsync(currentPage: 1, _maxRecommendedCourses);

                await FillTeacherInfo(courses.Items);

                _cacheService.SetData(cacheKey, courses, DateTimeOffset.Now.AddMinutes(CacheManager.RecommendedCourses.ExpireTimeInMinutes));
                LogInfo("End", method);
                return courses;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        private async Task<(List<Guid>, List<int>)> GetEnrolledCoursesAndCategories()
        {
            UserInfoState currentUser = GetCurrentUser();

            var enrolledCourses = currentUser == null
                ? []
                : await _context.CourseEnrollments
                    .Where(x => x.StudentId == currentUser.UserId)
                    .Select(x => new
                    {
                        x.CourseId,
                        x.Course.CategoryId
                    })
                    .ToListAsync();

            if (enrolledCourses.Count == 0)
            {
                return ([], []);
            }

            var enrolledCourseIds = enrolledCourses.Select(x => x.CourseId).ToList();
            var enrolledCourseCategories = enrolledCourses.Select(x => x.CategoryId).Distinct().ToList();

            return (enrolledCourseIds, enrolledCourseCategories);
        }

        private async Task FillTeacherInfo(List<CourseDetailWithTeacherDto> courses)
        {
            var teacherIds = courses.Select(x => x.Teacher.Id).Distinct().ToList();
            var teachers = await _grpcUserService.GetListOfTeachers(teacherIds);
            Dictionary<int, GrpcServices.SimpleUserResponse> teachersDict = teachers.Users.ToDictionary(x => x.Id, x => x);

            foreach (var course in courses)
            {
                if (teachersDict.TryGetValue(course.Teacher.Id, out var teacher))
                {
                    course.Teacher.FullName = teacher.FullName;
                    course.Teacher.AvatarURL = teacher.AvatarURL;
                }
            }
        }
    }
}