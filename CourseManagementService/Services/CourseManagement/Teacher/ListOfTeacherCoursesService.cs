using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
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

        public async Task<PaginatedList<CourseDto>> GetOwnCourses(CourseSearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);

                PaginatedList<CourseDto> courses = await _context.Courses
                    .Where(x => x.TeacherId == _appStateService.UserInfo.UserId
                        && (!searchCondition.CategoryId.HasValue || x.CategoryId == searchCondition.CategoryId)
                        && (string.IsNullOrEmpty(searchCondition.Keyword) 
                            || EF.Functions.ILike(x.Name, $"%{searchCondition.Keyword}%")
                            || EF.Functions.ILike(x.BriefDescription, $"%{searchCondition.Keyword}%")
                            || EF.Functions.ILike(x.DetailedDescription, $"%{searchCondition.Keyword}%"))
                    )
                    .OrderByDescending(x => x.UpdatedAt)
                    .Select(x => new CourseDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        BriefDescription = x.BriefDescription,
                        DetailedDescription = x.DetailedDescription,
                        ThumbnailURL = x.ThumbnailURL,
                        Price = x.Price,
                        CurrencyCode = x.Currency.Code,
                        Type = new LookupDto()
                        {
                            Id = EnumHelper.ConvertEnumToInt(x.Type).ToString(),
                            Name = x.Type.ToString(),
                        },
                        Category = new LookupDto()
                        {
                            Id = x.CategoryId.ToString(),
                            Name = x.Category.Name,
                        },
                        Tags = x.Tags.Select(t => new LookupDto()
                        {
                            Id = t.TagId.ToString(),
                            Name = t.Tag.Name,
                        })
                        .ToList(),
                        TotalStudents = x.Enrollments.Count,

                        // TODO: Count total lessons and total minutes
                        TotalLessons = 0,
                        TotalMinutes = 0,
                    })
                    .ToPaginatedListAsync(currentPage: searchCondition.CurrentPage, pageSize: searchCondition.PageSize);

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