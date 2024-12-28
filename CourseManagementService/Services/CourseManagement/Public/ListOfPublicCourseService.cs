using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.AppState.Schemas;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CategoryManagement.Schemas;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc.UserService;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Public
{
    public interface IListOfPublicCourseService
    {
        /// <summary>
        /// Get courses by keyword
        /// <para>Created at: 2024/10/22</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="condition">keyword to search</param>
        public Task<ListOfSearchItems> GetCoursesByKeyword(SearchCondition condition);

        /// <summary>
        /// Get courses by category
        /// <para>Created at: 2024/10/22</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <param name="condition">Search condition</param>
        /// <returns></returns>
        public Task<PaginatedList<CourseDetailWithTeacherDto>> GetCoursesByCategory(int categoryId, PublicCourseSearchWithoutCategoryCondition condition);

        /// <summary>
        /// Get courses by category
        /// <para>Created at: 2024/10/22</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="condition">Search condition</param>
        /// <returns></returns>
        public Task<PaginatedList<CourseDetailWithTeacherDto>> GetCoursesByCategory(PublicCourseSearchCategoryCondition condition);

        /// <summary>
        /// Get popular courses
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<PaginatedList<CourseDetailWithTeacherDto>> GetPopularCourses();

        /// <summary>
        /// Get recommended courses
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
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

        public async Task<ListOfSearchItems> GetCoursesByKeyword(SearchCondition condition)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var cacheKey = CacheManager.CourseSearch.Key(condition.Keyword, condition.CurrentPage, condition.PageSize);
                var cacheValue = _cacheService.GetData<ListOfSearchItems>(cacheKey);
                if (cacheValue != null)
                {
                    LogInfo("End", method);
                    return cacheValue;
                }

                var listOfSearchItems = new ListOfSearchItems();

                Task<List<CourseSearchItem>> coursesTask = _context.Courses
                    .Where(c => !condition.CategoryId.HasValue || c.CategoryId == condition.CategoryId.Value)
                    .Where(c => EF.Functions.ILike(c.Name, $"%{condition.Keyword}%"))
                    .OrderByDescending(c => c.Enrollments.Count)
                    .ThenByDescending(c => c.UpdatedAt)
                    .Select(c => new CourseSearchItem()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ThumbnailURL = c.ThumbnailURL,
                        TotalStudents = c.Enrollments.Count,
                        TotalLessons = c.Chapters.Sum(x => x.Lessons.Count),
                        TotalSecondsPerChapter = c.Chapters.Select(x => x.Lessons.Sum(y => y.DurationInSeconds)).ToList(),
                        Teacher = new TeacherDetail()
                        {
                            Id = c.TeacherId
                        }
                    })
                    .Skip((condition.CurrentPage - 1) * condition.PageSize)
                    .Take(condition.PageSize)
                    .ToListAsync();

                var teachersTask = _grpcUserService.GetTeachersByName(condition.Keyword);
                await Task.WhenAll(coursesTask, teachersTask);

                await FillTeacherInfo(coursesTask.Result);

                listOfSearchItems.Courses = coursesTask.Result;
                listOfSearchItems.Teachers = teachersTask.Result.Users.Select(x => new TeacherSearchItem()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    AvatarURL = x.AvatarURL
                })
                .ToList();

                _cacheService.SetData(cacheKey, listOfSearchItems,
                    DateTimeOffset.Now.AddMinutes(CacheManager.CourseSearch.ExpireTimeInMinutes));

                LogInfo("End", method);
                return listOfSearchItems;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<PaginatedList<CourseDetailWithTeacherDto>> GetPopularCourses()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                string cacheKey = "";
                UserInfoState currentUser = GetCurrentUser();
                (List<Guid> enrolledCourseIds, List<int> enrolledCourseCategories) = await GetEnrolledCoursesAndCategories();

                if (currentUser == null || enrolledCourseIds.Count == 0)
                {
                    cacheKey = CacheManager.PopularCourses.Key(0);
                }
                else
                {
                    cacheKey = CacheManager.PopularCourses.Key(currentUser.UserId);
                }
                var cacheValue = _cacheService.GetData<PaginatedList<CourseDetailWithTeacherDto>>(cacheKey);

                if (cacheValue != null)
                {
                    LogInfo("End", method);
                    return cacheValue;
                }

                var courses = await _context.Courses
                    .Where(x => !enrolledCourseIds.Contains(x.Id))
                    .OrderByDescending(x => x.Enrollments.Count)
                    .ThenByDescending(x => enrolledCourseCategories.Contains(x.CategoryId)) // Sắp xếp theo category đã tham gia
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(c => new CourseDetailWithTeacherDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Price = c.Price,
                        CurrencyCode = c.Currency.Code,
                        Type = new LookupDto()
                        {
                            Id = EnumHelper.ConvertEnumToInt(c.Type).ToString(),
                            Name = c.Type.ToString()
                        },
                        Category = new CategoryDto()
                        {
                            Id = c.Category.Id,
                            Name = c.Category.Name,
                            WebIconInfo = new IconInfoDto()
                            {
                                Icon = c.Category.WebIconInfo.Icon,
                                Color = c.Category.WebIconInfo.Color
                            },
                            MobileIconInfo = new IconInfoDto()
                            {
                                Icon = c.Category.MobileIconInfo.Icon,
                                Color = c.Category.MobileIconInfo.Color
                            }
                        },
                        Tags = c.Tags.Select(x => new LookupDto()
                        {
                            Id = x.Tag.Id.ToString(),
                            Name = x.Tag.Name
                        })
                        .ToList(),
                        ThumbnailURL = c.ThumbnailURL,
                        TotalStudents = c.Enrollments.Count,
                        TotalLessons = c.Chapters.Sum(x => x.Lessons.Count),
                        TotalSecondsByChapter = c.Chapters.Select(x => x.Lessons.Sum(y => y.DurationInSeconds)).ToList(),
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

                string cacheKey = "";
                UserInfoState currentUser = GetCurrentUser();
                (List<Guid> enrolledCourseIds, List<int> enrolledCourseCategories) = await GetEnrolledCoursesAndCategories();
                if (currentUser == null || enrolledCourseIds.Count == 0)
                {
                    cacheKey = CacheManager.RecommendedCourses.Key(0);
                }
                else
                {
                    cacheKey = CacheManager.RecommendedCourses.Key(currentUser.UserId);
                }

                var cacheValue = _cacheService.GetData<PaginatedList<CourseDetailWithTeacherDto>>(cacheKey);
                if (cacheValue != null)
                {
                    LogInfo("End", method);
                    return cacheValue;
                }

                // (List<Guid> enrolledCourseIds, List<int> enrolledCourseCategories) = await GetEnrolledCoursesAndCategories();
                var courses = await _context.Courses
                    .Where(x => !enrolledCourseIds.Contains(x.Id))
                    .OrderByDescending(x => enrolledCourseCategories.Contains(x.CategoryId)) // Sắp xếp theo category đã tham gia
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(c => new CourseDetailWithTeacherDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Price = c.Price,
                        CurrencyCode = c.Currency.Code,
                        Type = new LookupDto()
                        {
                            Id = EnumHelper.ConvertEnumToInt(c.Type).ToString(),
                            Name = c.Type.ToString()
                        },
                        Category = new CategoryDto()
                        {
                            Id = c.Category.Id,
                            Name = c.Category.Name,
                            WebIconInfo = new IconInfoDto()
                            {
                                Icon = c.Category.WebIconInfo.Icon,
                                Color = c.Category.WebIconInfo.Color
                            },
                            MobileIconInfo = new IconInfoDto()
                            {
                                Icon = c.Category.MobileIconInfo.Icon,
                                Color = c.Category.MobileIconInfo.Color
                            }
                        },
                        Tags = c.Tags.Select(x => new LookupDto()
                        {
                            Id = x.Tag.Id.ToString(),
                            Name = x.Tag.Name
                        })
                        .ToList(),
                        ThumbnailURL = c.ThumbnailURL,
                        TotalStudents = c.Enrollments.Count,
                        TotalLessons = c.Chapters.Sum(x => x.Lessons.Count),
                        TotalSecondsByChapter = c.Chapters.Select(x => x.Lessons.Sum(y => y.DurationInSeconds)).ToList(),
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

        public Task<PaginatedList<CourseDetailWithTeacherDto>> GetCoursesByCategory(int categoryId, PublicCourseSearchWithoutCategoryCondition condition)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var searchConditionWithCategory = new PublicCourseSearchCategoryCondition()
                {
                    CategoryId = categoryId,
                    Keyword = condition.Keyword,
                    CurrentPage = condition.CurrentPage,
                    PageSize = condition.PageSize,
                    SortBy = condition.SortBy
                };

                return GetCoursesByCategory(searchConditionWithCategory);
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
            finally
            {
                LogInfo("End", method);
            }
        }

        public async Task<PaginatedList<CourseDetailWithTeacherDto>> GetCoursesByCategory(PublicCourseSearchCategoryCondition condition)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var currentUser = GetCurrentUser();
                var cacheKey = CacheManager.CourseSearchByCategory.Key(condition.CategoryId ?? 0, currentUser?.UserId ?? 0, condition);
                var cacheValue = _cacheService.GetData<PaginatedList<CourseDetailWithTeacherDto>>(cacheKey);

                if (cacheValue != null)
                {
                    return cacheValue;
                }

                PaginatedList<CourseDetailWithTeacherDto> courses = await _context.Courses
                .Where(c => (!condition.CategoryId.HasValue || c.CategoryId == condition.CategoryId)
                    && (string.IsNullOrEmpty(condition.Keyword)
                        || EF.Functions.ILike(c.Name, $"%{condition.Keyword}%")
                        || EF.Functions.ILike(c.Description, $"%{condition.Keyword}%")))
                .SingleSort(condition)
                .Select(c => new CourseDetailWithTeacherDto()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CoreValues = c.CoreValues,
                    Prerequisites = c.Prerequisites,
                    Price = c.Price,
                    CurrencyCode = c.Currency.Code,
                    Type = new LookupDto()
                    {
                        Id = EnumHelper.ConvertEnumToInt(c.Type).ToString(),
                        Name = c.Type.ToString()
                    },
                    Category = new CategoryDto()
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                        WebIconInfo = new IconInfoDto()
                        {
                            Icon = c.Category.WebIconInfo.Icon,
                            Color = c.Category.WebIconInfo.Color
                        },
                        MobileIconInfo = new IconInfoDto()
                        {
                            Icon = c.Category.MobileIconInfo.Icon,
                            Color = c.Category.MobileIconInfo.Color
                        }
                    },
                    Tags = c.Tags.Select(x => new LookupDto()
                    {
                        Id = x.Tag.Id.ToString(),
                        Name = x.Tag.Name
                    })
                    .ToList(),
                    ThumbnailURL = c.ThumbnailURL,
                    TotalStudents = c.Enrollments.Count,
                    TotalLessons = c.Chapters.Sum(x => x.Lessons.Count),
                    TotalSecondsByChapter = c.Chapters.Select(x => x.Lessons.Sum(y => y.DurationInSeconds)).ToList(),
                    Teacher = new TeacherDetail()
                    {
                        Id = c.TeacherId
                    },
                    IsRegistered = currentUser != null && (
                        c.TeacherId == currentUser.UserId || c.Enrollments.Any(x => x.StudentId == currentUser.UserId)
                    ),
                    AverageRating = c.Ratings.Any() ? c.Ratings.Average(x => x.Rating) : null,
                })
                .ToPaginatedListAsync(condition.CurrentPage, condition.PageSize);

                await FillTeacherInfo(courses.Items);

                _cacheService.SetData(cacheKey, courses, DateTimeOffset.Now.AddMinutes(CacheManager.CourseSearchByCategory.ExpireTimeInMinutes));
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

        private async Task FillTeacherInfo<T>(List<T> courses) where T : ICourseWithTeacher
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
                    course.Teacher.Email = teacher.Email;
                }
            }
        }
    }
}