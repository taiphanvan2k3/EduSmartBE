using CourseManagementService.Services.RatingManagement;
using CourseManagementService.Services.RatingManagement.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Authorize]
    [Route("course-service/api", Order = 12)]
    [ApiController]
    public class RatingManagementController(IRatingDetailService ratingDetailService) : BaseController
    {
        private readonly IRatingDetailService _ratingDetailService = ratingDetailService
            ?? throw new ArgumentException(nameof(ratingDetailService));

        /// <summary>
        /// Make a rating for a lesson
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="courseId">Course Id</param>
        /// <param name="courseRatingCreate">Course rating information</param>
        /// <returns></returns>
        [HttpPost("courses/{courseId}/ratings")]
        public async Task<IActionResult> CreateLessonRating([FromRoute] Guid courseId, CourseRatingCreateDto courseRatingCreate)
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
        /// Make a rating for a lesson
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
    }
}