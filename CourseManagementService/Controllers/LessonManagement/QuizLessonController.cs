using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.LessonManagement.QuizLesson;
using CourseManagementService.Services.LessonManagement.QuizLesson.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.LessonManagement
{
    [Route("course-service/api/quiz-lessons", Order = 7)]
    [ApiController]
    public class QuizLessonController(IQuizLessonDetailService quizLessonDetailService) : BaseController
    {
        private readonly IQuizLessonDetailService _quizLessonDetailService = quizLessonDetailService
            ?? throw new ArgumentNullException(nameof(quizLessonDetailService));

        /// <summary>
        /// Get a quiz lesson by id
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of quiz lesson</param>
        [HttpGet("{id}")]
        [Filters.Auth]
        [ProducesResponseType(typeof(QuizLessonDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuizLessonById(Guid id)
        {
            var responseInfo = await _quizLessonDetailService.GetQuizLessonDetail(id);
            return HandleResponseInfo(responseInfo, resourceName: "lesson");
        }

        /// <summary>
        /// Create a new quiz lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="quizLessonCreateDto">Quiz lesson information is need for create</param>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page) 
        /// </remarks>
        [HttpPost]
        [Filters.Auth(Roles = "Teacher")]
        public async Task<IActionResult> CreateLesson(QuizLessonCreateDto quizLessonCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _quizLessonDetailService.CreateQuizLesson(quizLessonCreateDto);
            return HandleResponseInfo(responseInfo, resourceName: "lesson");
        }

        /// <summary>
        /// Update a quiz lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page) 
        /// </remarks>
        [HttpPut("{id}")]
        [Filters.Auth(Roles = "Teacher")]
        public async Task<IActionResult> UpdateLesson(Guid id, QuizLessonUpdateDto quizLessonUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _quizLessonDetailService.UpdateQuizLesson(id, quizLessonUpdateDto);
            return HandleResponseInfo(responseInfo, resourceName: "lesson");
        }

        /// <summary>
        /// Delete a quiz lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of quiz lesson is need for delete</param>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page) 
        /// </remarks>
        [HttpDelete("{id}")]
        [Filters.Auth(Roles = "Teacher")]
        public async Task<IActionResult> DeleteLesson(Guid id)
        {
            var responseInfo = await _quizLessonDetailService.DeleteQuizLesson(id);
            return HandleResponseInfo(responseInfo, resourceId: id.ToString());
        }
    }
}