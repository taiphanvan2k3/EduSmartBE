using CourseManagementService.Services.DiscussionManagement.Discussions;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.DiscussionManagement
{
    [Route("course-service/api/discussions", Order = 10)]
    [ApiController]
    [Authorize]
    public class DiscussionController(IDiscussionDetailService discussionDetailService) : BaseController
    {
        private readonly IDiscussionDetailService _discussionDetailService = discussionDetailService
            ?? throw new ArgumentNullException(nameof(discussionDetailService));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscussion(Guid id)
        {
            var responseInfo = await _discussionDetailService.GetDiscussion(id);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscussion([FromBody] DiscussionCreateDto discussionCreate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _discussionDetailService.CreateDiscussion(discussionCreate);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscussion(Guid id, [FromBody] DiscussionUpdateDto discussionUpdate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _discussionDetailService.UpdateDiscussion(id, discussionUpdate);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }
    }
}