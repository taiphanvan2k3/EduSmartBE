using Microsoft.AspNetCore.Mvc;
using UserService.Commons.Helpers;
using UserService.Commons.Schemas;
using UserService.Services.Users;
using UserService.Services.Users.Schemas;

namespace UserService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(IListOfUsersService listOfUsersService) : ControllerBase
    {
        private readonly IListOfUsersService _listOfUsersService = listOfUsersService
            ?? throw new ArgumentNullException(nameof(listOfUsersService));

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
    }
}