using CourseManagementService.Services.Medias;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course-service/api/lessons")]
    [ApiController]
    public class LessonController(IVideoService videoService) : ControllerBase
    {
        private readonly IVideoService _videoService = videoService
            ?? throw new ArgumentNullException(nameof(videoService));

        [HttpPost("upload-video")]
        // 100MB
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> UploadVideoAsync(IFormFile file)
        {
            var responseInfo = await _videoService.UploadVideoAsync(file);
            return Ok(responseInfo);
        }
    }
}