using Microsoft.AspNetCore.Mvc;
using UserService.Commons.Helpers;
using UserService.Commons.Schemas;
using UserService.Services.Users;
using UserService.Services.Users.Schemas;

namespace UserService.Controllers
{
    [Route("user-service/api/users")]
    [ApiController]
    public class UserController(IListOfUsersService listOfUsersService,
        IUserService userService) : ControllerBase
    {
        private readonly IListOfUsersService _listOfUsersService = listOfUsersService
            ?? throw new ArgumentNullException(nameof(listOfUsersService));

        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        [Filters.Auth(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] SearchCondition searchCondition)
        {
            try
            {
                var users = await _listOfUsersService.GetUsers(searchCondition);
                return Ok(users);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng
        /// <para>Created at: 2024/10/08</para>
        /// <para>Created by: ManhTD</para> 
        /// </summary>
        /// <param name="userProfileUpdateRequest">Thông tin cập nhật người dùng</param>
        /// <returns>Thông tin hồ sơ đã được cập nhật</returns>
        /// <remarks>
        /// API này được sử dụng để cập nhật thông tin hồ sơ của người dùng.
        /// 
        /// Response codes:
        /// 
        ///     200 - Thành công, hồ sơ người dùng đã được cập nhật.
        ///     400 - Dữ liệu gửi lên không hợp lệ.
        ///     401 - Người dùng cần đăng nhập để thực hiện thao tác này.
        ///     403 - Người dùng không có quyền truy cập tài nguyên này.
        ///     500 - Lỗi hệ thống khi xử lý yêu cầu.
        /// </remarks>
        /// <response code="200">Hồ sơ người dùng đã được cập nhật thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ. Có lỗi trong dữ liệu đầu vào.</response>
        /// <response code="401">Người dùng chưa đăng nhập.</response>
        /// <response code="403">Người dùng không có quyền thực hiện thao tác này.</response>
        /// <response code="500">Lỗi nội bộ hệ thống.</response>
        [Filters.Auth]
        [HttpPut("me/profile")]
        [ProducesResponseType(typeof(UserProfileUpdateRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfileUser([FromForm] UserProfileUpdateRequest userProfileUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()));
            }
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                var responseInfo = await _userService.UpdateProfileUser(int.Parse(userId), userProfileUpdateRequest);
                if (responseInfo.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new
                    {
                        userInfo = responseInfo.Data["userInfo"]
                    });
                }
                else
                {
                    return StatusCode(responseInfo.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                        responseInfo.StatusCode, responseInfo.Error, responseInfo.Message));
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        [HttpGet("me")]
        [Filters.Auth]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserDetail()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var user = await _userService.GetUserDetail(Int32.Parse(userId));
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}