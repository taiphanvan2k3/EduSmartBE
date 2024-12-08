using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CategoryManagement.Schemas;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Teacher
{
    public interface IListOfTeacherCoursesService
    {
        /// <summary>
        /// Get all courses that teacher created
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/07</para>
        /// </summary>
        public Task<PaginatedList<CourseDto>> GetOwnCourses(CourseSearchCondition searchCondition);
    }

    public class ListOfTeacherCoursesService(IServiceProvider serviceProvider, ILogger<TeacherCourseDetailService> logger)
        : BaseService(serviceProvider, logger), IListOfTeacherCoursesService
    {
        private readonly string _serviceName = nameof(ListOfTeacherCoursesService);
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError("ICacheService"));

        public async Task<PaginatedList<CourseDto>> GetOwnCourses(CourseSearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);

                var cacheKey = CacheManager.OwnedCourses.Key(_appStateService.UserInfo.UserId, searchCondition.CurrentPage, searchCondition.PageSize);
                var cachedCourses = _cacheService.GetData<PaginatedList<CourseDto>>(cacheKey);
                if (cachedCourses != null)
                {
                    LogInfo("End (Cache hit)", methodName);
                    return cachedCourses;
                }

                PaginatedList<CourseDto> courses = await _context.Courses
                    .Where(x => x.TeacherId == _appStateService.UserInfo.UserId
                        && (!searchCondition.CategoryId.HasValue || x.CategoryId == searchCondition.CategoryId)
                        && (string.IsNullOrEmpty(searchCondition.Keyword)
                            || EF.Functions.ILike(x.Name, $"%{searchCondition.Keyword}%")
                            || EF.Functions.ILike(x.Description, $"%{searchCondition.Keyword}%"))
                    )
                    .OrderByDescending(x => x.UpdatedAt)
                    .Select(x => new CourseDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        CoreValues = x.CoreValues,
                        ThumbnailURL = x.ThumbnailURL,
                        Price = x.Price,
                        CurrencyCode = x.Currency.Code,
                        Type = new LookupDto()
                        {
                            Id = EnumHelper.ConvertEnumToInt(x.Type).ToString(),
                            Name = x.Type.ToString(),
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
                        Tags = x.Tags.Select(t => new LookupDto()
                        {
                            Id = t.TagId.ToString(),
                            Name = t.Tag.Name,
                        })
                        .ToList(),
                        TotalStudents = x.Enrollments.Count,
                        TotalLessons = x.Chapters.Sum(c => c.Lessons.Count),
                        TotalSecondsByChapter = x.Chapters.Select(c => c.Lessons.Sum(l => l.DurationInSeconds)).ToList(),
                        IsPublished = x.IsPublished,
                        IsRegistered = true,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    })
                    .ToPaginatedListAsync(currentPage: searchCondition.CurrentPage, pageSize: searchCondition.PageSize);

                _cacheService.SetData(cacheKey, courses, DateTimeOffset.Now.AddMinutes(CacheManager.OwnedCourses.ExpireTimeInMinutes));

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, methodName);
                return courses;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }
    }
}