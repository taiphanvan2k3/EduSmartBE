using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Commons.Helpers;
using UserService.Services.UserInfos;
using UserService.Services.UserInfos.Schemas;

namespace UserService.Controllers
{
    [Route("api/user-infos")]
    [ApiController]
    [Authorize]
    public class UserInfoController(IUserInfoService userInfoService) : ControllerBase
    {
        private readonly IUserInfoService _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));

        /// <summary>
        /// Update profile user
        /// <para>Created at: 2024/10/08</para>
        /// <para>Created by: ManhTD</para> 
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Profile updated</returns>
        /// <response code="200">Update success</response>
        /// <response code="400">Yêu cầu gửi lên không đúng định dạng</response>
        /// <response code="500">Lỗi khi có exception</response>
        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfileUser([FromForm] UserProfileUpdateRequest userProfileUpdateRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()));
            }
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                var result = await _userInfoService.UpdateProfileUser(Int32.Parse(userId), userProfileUpdateRequest.UserInfo, userProfileUpdateRequest.File);
                return Ok(new
                {
                    imageInfo = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}