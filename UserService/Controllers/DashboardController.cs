using Microsoft.AspNetCore.Mvc;
using UserService.Commons.Helpers;
using UserService.Services.Users;
using UserService.Services.Users.Schemas;

namespace UserService.Controllers
{
    [Route("user-service/api/dashboard")]
    [ApiController]
    public class DashboardController(IUserService userService): ControllerBase
    {
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        [Filters.Auth(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var dashboard = await _userService.GetYearlyDataForDashboard();
                return Ok(dashboard);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}