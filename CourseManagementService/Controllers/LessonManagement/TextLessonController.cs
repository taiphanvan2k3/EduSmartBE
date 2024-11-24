using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.LessonManagement.TextLesson;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.LessonManagement
{
    [Route("course-service/api/text-lessons", Order = 7)]
    [ApiController]
    public class TextLessonController(ITextLessonDetailService textLessonDetailService) : BaseController
    {
        private readonly ITextLessonDetailService _textLessonDetailService = textLessonDetailService
            ?? throw new ArgumentNullException(nameof(textLessonDetailService));

        /// <summary>
        /// Get a text lesson by id
        /// <para>Created at: 2024/11/12</para> 
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of text lesson</param>
        [HttpGet("{id}")]
        [Filters.Auth]
        [ProducesResponseType(typeof(LessonDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTextLessonById(Guid id)
        {
            var responseInfo = await _textLessonDetailService.GetTextLessonDetail(id);
            return HandleResponseInfo(responseInfo, resourceName: "lesson");
        }

        /// <summary>
        /// Create a new text lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="textLessonCreateDto">text lesson information is need for create</param>
        [Filters.Auth(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateLesson(TextLessonCreateUpdateDto textLessonCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _textLessonDetailService.CreateTextLesson(textLessonCreateDto);
            return HandleResponseInfo(responseInfo, resourceName: "lesson");
        }

        /// <summary>
        /// Update a text lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [Filters.Auth(Roles = "Teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(Guid id, TextLessonCreateUpdateDto textLessonCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _textLessonDetailService.UpdateTextLesson(id, textLessonCreateDto);
            return HandleResponseInfo(responseInfo, resourceName: "lesson");
        }

        /// <summary>
        /// Delete a text lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of text lesson is need for delete</param>
        [Filters.Auth(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(Guid id)
        {
            var responseInfo = await _textLessonDetailService.DeleteTextLesson(id);
            return HandleResponseInfo(responseInfo, resourceId: id.ToString());
        }
    }
}