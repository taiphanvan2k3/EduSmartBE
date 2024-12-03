using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Enumerations;
using CourseManagementService.Extensions;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Teacher;
using CourseManagementService.Services.Grpc.UserService;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.RatingManagement.Schemas;
using Microsoft.EntityFrameworkCore;
using static CourseManagementService.Common.Constants;

namespace CourseManagementService.Services.RatingManagement
{
    public interface IListOfRatingsService
    {
        /// <summary>
        /// Get list of course ratings
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of course</param>
        /// <param name="paramsSearch">Pagination information</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetListOfCourseRatings(Guid courseId, ParamsSearch paramsSearch);

        /// <summary>
        /// Get list of overall lesson ratings. This API is only used for teacher
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of course</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetListOfOverallLessonRatingsInCourse(Guid courseId);

        /// <summary>
        /// Get the list of detailed lesson ratings in a lesson
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> GetListOfLessonRatings(LessonRatingSearchCondition searchCondition);
    }

    public class ListOfRatingsService(IServiceProvider serviceProvider, ILogger<ListOfRatingsService> logger)
        : BaseService(serviceProvider, logger), IListOfRatingsService
    {
        private readonly IMapper _mapper = serviceProvider.GetService<IMapper>()
            ?? throw new ArgumentNullException(nameof(IMapper));
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new ArgumentNullException(nameof(ICacheService));
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(nameof(IGrpcUserService));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new ArgumentNullException(nameof(ILessonBaseDetailService));
        private readonly ITeacherCourseDetailService _teacherCourseDetailService = serviceProvider.GetService<ITeacherCourseDetailService>()
            ?? throw new ArgumentNullException(nameof(ITeacherCourseDetailService));

        public async Task<ResponseInfo> GetListOfCourseRatings(Guid courseId, ParamsSearch paramsSearch)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var coursePermission = await _lessonBaseDetailService.CheckCoursePermission(courseId, currentUser.UserId);
                if (!coursePermission.IsSuccess)
                {
                    return coursePermission;
                }

                PaginatedList<CourseRatingDetail> courseRatings = await _context.CourseRatings
                    .Where(x => x.CourseId == courseId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<CourseRatingDetail>(_mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(paramsSearch.CurrentPage, paramsSearch.PageSize);

                var userIds = courseRatings.Items.Select(x => x.User.Id).Distinct().ToList();
                var userInfos = await _grpcUserService.GetListOfUsers(userIds);

                var teacherIdInCourse = await _teacherCourseDetailService.GetTeacherIdOfCourse(courseId);

                foreach (var courseRating in courseRatings.Items)
                {
                    var userTemp = userInfos.FirstOrDefault(x => x.Id == courseRating.User.Id);
                    if (userTemp != null)
                    {
                        courseRating.User = userTemp;
                        courseRating.User.RoleInCourse = teacherIdInCourse == courseRating.User.Id
                            ? RoleInCourse.TEACHER
                            : RoleInCourse.STUDENT;
                    }
                }

                var courseRatingOverall = await _context.CourseRatings
                    .Where(x => x.CourseId == courseId)
                    .GroupBy(x => x.CourseId)
                    .Select(x => new CourseRatingOverall
                    {
                        CourseId = x.Key,
                        OverallRating = x.Average(x => x.Rating),
                        TotalRatings = x.Count()
                    })
                    .FirstOrDefaultAsync();

                var listOfCourseRatings = new ListOfCourseRatings
                {
                    CourseRatingOverall = courseRatingOverall,
                    Ratings = courseRatings
                };

                return new ResponseInfo("courseRatings", listOfCourseRatings);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }

        public async Task<ResponseInfo> GetListOfOverallLessonRatingsInCourse(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var teacherIdInCourse = await _teacherCourseDetailService.GetTeacherIdOfCourse(courseId);
                if (teacherIdInCourse != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this resource");
                }

                var chapterWithRatings = await _context.Chapters
                    .Where(c => c.CourseId == courseId)
                    .Where(c => c.Lessons.Any(l => l.Ratings.Count > 0))
                    .OrderBy(c => c.Order)
                    .Select(c => new ChapterWithRatings()
                    {
                        Order = c.Order,
                        Chapter = c.Name,
                        LessonRatings = c.Lessons
                            .SelectMany(l => l.Ratings)
                            .GroupBy(r => r.LessonId)
                            .Select(r => new LessonRatingOverall()
                            {
                                LessonId = r.Key,
                                LessonName = r.First().Lesson.Title,
                                LikeCount = r.Count(x => x.IsLike),
                                DislikeCount = r.Count(x => !x.IsLike),
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return new ResponseInfo("lessonRatings", chapterWithRatings);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }

        public async Task<ResponseInfo> GetListOfLessonRatings(LessonRatingSearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var lessonPermission = await _lessonBaseDetailService.CheckLessonPermission(searchCondition.LessonId, currentUser.UserId);
                if (!lessonPermission.IsSuccess)
                {
                    return lessonPermission;
                }

                var lessonRatings = await _context.LessonRatings
                    .Where(lr => lr.LessonId == searchCondition.LessonId && (searchCondition.RatingFilter == LessonRatingFilter.All
                        || (searchCondition.RatingFilter == LessonRatingFilter.Like && lr.IsLike)
                        || (searchCondition.RatingFilter == LessonRatingFilter.Dislike && !lr.IsLike)))
                    .OrderByDescending(lr => lr.CreatedAt)
                    .ProjectTo<LessonRatingDetail>(_mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                var userIds = lessonRatings.Items.Select(x => x.User.Id).Distinct().ToList();
                var userInfos = await _grpcUserService.GetListOfUsers(userIds);

                foreach (var lessonRating in lessonRatings.Items)
                {
                    var userTemp = userInfos.FirstOrDefault(x => x.Id == lessonRating.User.Id);
                    if (userTemp != null)
                    {
                        lessonRating.User = userTemp;
                    }
                }

                return new ResponseInfo("lessonRatings", lessonRatings);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }
    }
}