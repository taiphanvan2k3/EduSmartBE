using CourseManagementService.Common;
using CourseManagementService.Services.BookmarkManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course-service/api", Order = 14)]
    [ApiController]
    [Authorize]
    public class BookmarkController(IBookmarkService bookmarkService) : BaseController
    {
        private readonly IBookmarkService _bookmarkService = bookmarkService
            ?? throw new ArgumentNullException(nameof(bookmarkService));

        /// <summary>
        /// Bookmark a lesson
        /// <para>Created at: 2024/12/05</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <returns></returns>
        [HttpPut("lessons/{lessonId}/bookmark")]
        public async Task<IActionResult> ToggleBookmark([FromRoute] Guid lessonId)
        {
            var responseInfo = await _bookmarkService.ToggleBookmark(lessonId);
            return HandleResponseInfo(responseInfo, resourceName: "bookmarkStatus");
        }

        /// <summary>
        /// Get list of bookmarked lesson ids by course id
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("courses/{courseId}/bookmarked-lessons")]
        public async Task<IActionResult> GetBookmarkedLessonsByCourse([FromRoute] Guid courseId)
        {
            var bookmarkedLessons = await _bookmarkService.GetBookmarkedLessonIdsAsBookmarks(courseId);
            return Ok(bookmarkedLessons);
        }
    }
}