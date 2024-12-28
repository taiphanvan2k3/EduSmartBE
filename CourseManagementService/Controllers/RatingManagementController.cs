using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.RatingManagement;
using CourseManagementService.Services.RatingManagement.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Authorize]
    [Route("course-service/api", Order = 12)]
    [ApiController]
    public class RatingManagementController(
        IListOfRatingsService listOfRatingsService,
        IRatingDetailService ratingDetailService) : BaseController
    {
        private readonly IListOfRatingsService _listOfRatingsService = listOfRatingsService
            ?? throw new ArgumentException(nameof(listOfRatingsService));
        private readonly IRatingDetailService _ratingDetailService = ratingDetailService
            ?? throw new ArgumentException(nameof(ratingDetailService));

        /// <summary>
        /// Get list of ratings of a course (call this API when access to course detail page)
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <param name="paramsSearch">Pagination information</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("courses/{courseId}/ratings")]
        public async Task<IActionResult> GetListOfCourseRatings([FromRoute] Guid courseId, [FromQuery] ParamsSearch paramsSearch)
        {
            var responseInfo = await _listOfRatingsService.GetListOfCourseRatings(courseId, paramsSearch);
            return HandleResponseInfo(responseInfo, resourceName: "courseRatings", isWrapperInObject: false);
        }

        /// <summary>
        /// Student uses this API to get their rating of a course
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <returns></returns>
        [HttpGet("courses/{courseId}/my-rating")]
        public async Task<IActionResult> GetMyCourseRating([FromRoute] Guid courseId)
        {
            var responseInfo = await _ratingDetailService.GetCourseRatingOfStudent(courseId);
            if (responseInfo.IsSuccess)
            {
                return Ok(new
                {
                    myRating = responseInfo.Data["myCourseRating"]
                });
            }
            else
            {
                return HandleResponseInfo(responseInfo);
            }
        }

        /// <summary>
        /// [Teacher Only] Get a list of chapters with their rated lessons in a course
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <remarks>
        /// NOTE:
        /// 
        /// This API retrieves a list of chapters in a course that have received ratings, 
        /// along with the lessons within those chapters that are rated.
        /// 
        /// Access is restricted to users with the "Teacher" role.
        /// 
        /// </remarks> 
        [Filters.Auth(Roles = "Teacher")]
        [HttpGet("courses/{courseId}/overall-lesson-ratings")]
        [ProducesResponseType(typeof(List<ChapterWithRatings>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListOfLessonRatings([FromRoute] Guid courseId)
        {
            var responseInfo = await _listOfRatingsService.GetListOfOverallLessonRatingsInCourse(courseId);
            return HandleResponseInfo(responseInfo, resourceName: "lessonRatings", isWrapperInObject: false);
        }

        /// <summary>
        /// Create an new rating for a course
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="courseId">Course Id</param>
        /// <param name="courseRatingCreate">Course rating information</param>
        /// <returns></returns>
        [HttpPost("courses/{courseId}/ratings")]
        public async Task<IActionResult> CreateCourseRating([FromRoute] Guid courseId, CourseRatingCreateDto courseRatingCreate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            courseRatingCreate.CourseId = courseId;
            var responseInfo = await _ratingDetailService.CreateCourseRating(courseRatingCreate);
            return HandleResponseInfo(responseInfo, resourceName: "courseRatingId");
        }

        /// <summary>
        /// Update a course rating
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <param name="id">Id of the rating</param>
        /// <param name="courseRatingUpdate">Course rating information</param>
        /// <returns></returns>
        [HttpPut("courses/{courseId}/ratings/{id}")]
        public async Task<IActionResult> UpdateCourseRating([FromRoute] Guid courseId,
            [FromRoute] Guid id, CourseRatingUpdateDto courseRatingUpdate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            courseRatingUpdate.CourseId = courseId;
            courseRatingUpdate.Id = id;

            var responseInfo = await _ratingDetailService.UpdateCourseRating(courseRatingUpdate);
            return HandleResponseInfo(responseInfo, resourceName: "courseRating");
        }

        /// <summary>
        /// Delete a rating of a course
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <param name="id">Id of the rating</param>
        /// <returns></returns>
        [HttpDelete("courses/{courseId}/ratings/{id}")]
        public async Task<IActionResult> DeleteCourseRating([FromRoute] Guid courseId, [FromRoute] Guid id)
        {
            var responseInfo = await _ratingDetailService.DeleteCourseRating(courseId, id);
            return HandleResponseInfo(responseInfo, resourceId: id.ToString());
        }

        /// <summary>
        /// Student uses this API to get their rating of a lesson
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="lessonId">Id of the lesson</param>
        /// <returns></returns>
        [HttpGet("lessons/{lessonId}/my-rating")]
        public async Task<IActionResult> GetMyLessonRating([FromRoute] Guid lessonId)
        {
            var responseInfo = await _ratingDetailService.GetLessonRatingOfStudent(lessonId);
            if (responseInfo.IsSuccess)
            {
                return Ok(new
                {
                    myRating = responseInfo.Data["myLessonRating"]
                });
            }
            else
            {
                return HandleResponseInfo(responseInfo);
            }
        }

        /// <summary>
        /// [Teacher Only] Teacher uses this API to get overall rating of a lesson such as like count, dislike count
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Id of the lesson</param>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the teacher role
        ///     
        /// </remarks> 
        [HttpGet("lessons/{lessonId}/overall-rating")]
        [ProducesResponseType(typeof(LessonRatingOverall), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOverallRatingInLesson([FromRoute] Guid lessonId)
        {
            var responseInfo = await _ratingDetailService.GetOverallLessonRating(lessonId);
            return HandleResponseInfo(responseInfo, resourceName: "overallLessonRating");
        }

        /// <summary>
        /// [Teacher Only] Teacher uses this API to get detailed ratings of a lesson
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Id of the lesson</param>
        /// <param name="lessonRatingSearchCondition">Search condition</param>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the teacher role
        ///     
        /// </remarks> 
        [Filters.Auth(Roles = "Teacher")]
        [HttpGet("lessons/{lessonId}/detailed-ratings")]
        [ProducesResponseType(typeof(LessonRatingOverall), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetailedRatingInLesson([FromRoute] Guid lessonId, [FromQuery] LessonRatingSearchCondition lessonRatingSearchCondition)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            lessonRatingSearchCondition.LessonId = lessonId;
            var responseInfo = await _listOfRatingsService.GetListOfLessonRatings(lessonRatingSearchCondition);
            return HandleResponseInfo(responseInfo, resourceName: "lessonRatings");
        }

        /// <summary>
        /// Create a rating for a lesson
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="lessonId">Lesson Id</param>
        /// <param name="lessonRatingCreateDto">Lesson rating information</param>
        /// <returns></returns>
        [HttpPost("lessons/{lessonId}/ratings")]
        public async Task<IActionResult> CreateLessonRating([FromRoute] Guid lessonId, LessonRatingCreateDto lessonRatingCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            lessonRatingCreateDto.LessonId = lessonId;
            var responseInfo = await _ratingDetailService.CreateLessonRating(lessonRatingCreateDto);
            return HandleResponseInfo(responseInfo, resourceName: "lessonRatingId");
        }

        /// <summary>
        /// Student update their rating for a lesson
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Id of the lesson</param>
        /// <param name="id">Id of the rating</param>
        /// <param name="lessonRatingUpdateDto">Lesson rating information</param>
        /// <returns></returns>
        [HttpPut("lessons/{lessonId}/ratings/{id}")]
        public async Task<IActionResult> UpdateLessonRating([FromRoute] Guid lessonId, [FromRoute] Guid id, [FromBody] LessonRatingUpdateDto lessonRatingUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            lessonRatingUpdateDto.LessonId = lessonId;
            lessonRatingUpdateDto.Id = id;

            var responseInfo = await _ratingDetailService.UpdateLessonRating(lessonRatingUpdateDto);
            return HandleResponseInfo(responseInfo, resourceName: "lessonRating");
        }

        /// <summary>
        /// Delete a rating of a lesson
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Id of the lesson</param>
        /// <param name="id">Id of the rating</param>
        /// <returns></returns>
        [HttpDelete("lessons/{lessonId}/ratings/{id}")]
        public async Task<IActionResult> DeleteLessonRating([FromRoute] Guid lessonId, [FromRoute] Guid id)
        {
            var responseInfo = await _ratingDetailService.DeleteLessonRating(lessonId, id);
            return HandleResponseInfo(responseInfo, resourceId: id.ToString());
        }
    }
}