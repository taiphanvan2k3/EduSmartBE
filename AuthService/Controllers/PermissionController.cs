using AuthService.Services.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthService.Services.Permission.Schemas.Screen;
using AuthService.Services.Permission.Schemas.Function;
using AuthService.Services.Permission.Schemas;
using AuthService.Binders;
using System.Security.Claims;

namespace AuthService.Controllers
{
    [Route("api/permission")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class PermissionController(IScreenService screenService, IFunctionService functionService,
        IPermissionService permissionService) : ControllerBase
    {
        private readonly IScreenService _screenService = screenService
            ?? throw new ArgumentNullException(nameof(screenService));

        private readonly IFunctionService _functionService = functionService
            ?? throw new ArgumentNullException(nameof(functionService));

        private readonly IPermissionService _permissionService = permissionService
            ?? throw new ArgumentNullException(nameof(permissionService));

        #region Screen actions

        [HttpGet("screens")]
        public async Task<IActionResult> GetListScreens()
        {
            try
            {
                var screens = await _screenService.GetListScreen();
                return Ok(screens);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.Message
                });
            }
        }

        [HttpPost("screens/create")]
        public async Task<IActionResult> CreateScreen(ScreenCreateDto screen)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid screen info"
                    });
                }

                var response = await _screenService.CreateScreen(screen);
                return CreatedAtAction(nameof(GetListScreens),
                    response.Data.TryGetValue("result", out var result) ? result : new { });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        [HttpPut("screens/update/{id}")]
        public async Task<IActionResult> UpdateScreen(string id, ScreenUpdateDto screen)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid screen info"
                    });
                }

                var response = await _screenService.UpdateScreen(id, screen);
                return Ok(response.Data.TryGetValue("result", out var result) ? result : new { });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        [HttpDelete("screens/delete/{id}")]
        public async Task<IActionResult> DeleteScreen(string id)
        {
            try
            {
                var response = await _screenService.DeleteScreen(id);
                if (response.StatusCode == StatusCodes.Status404NotFound)
                {
                    return NotFound(new
                    {
                        message = response.Message
                    });
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        #endregion

        #region Function actions

        [HttpGet("functions")]
        public async Task<IActionResult> GetListOfFunctions([ModelBinder(BinderType = typeof(PagingInfoModelBinder))] PagingInfo pagingInfo)
        {
            try
            {
                if (pagingInfo.PageIndex < 1 || pagingInfo.PageSize < 1)
                {
                    return BadRequest(new
                    {
                        message = "Invalid paging info"
                    });
                }

                var response = await _functionService.GetListOfFunctions(pagingInfo);
                return Ok(response.Data.TryGetValue("result", out var result) ? result : new { });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        [HttpPost("functions/create")]
        public async Task<IActionResult> CreateFunction(FunctionCreateDto function)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid function info"
                    });
                }

                var response = await _functionService.CreateFunction(function);
                if (response.StatusCode == StatusCodes.Status404NotFound)
                {
                    return NotFound(new
                    {
                        message = response.Message
                    });
                }
                return CreatedAtAction(nameof(GetListOfFunctions),
                    response.Data.TryGetValue("result", out var result) ? result : new { });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        [HttpPut("functions/update/{id}")]
        public async Task<IActionResult> UpdateFunction(string id, FunctionUpdateDto function)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid function info"
                    });
                }

                var response = await _functionService.UpdateFunction(id, function);
                if (response.StatusCode == StatusCodes.Status404NotFound)
                {
                    return NotFound(new
                    {
                        message = response.Message
                    });
                }
                return Ok(response.Data.TryGetValue("result", out var result) ? result : new { });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        [HttpDelete("functions/delete/{id}")]
        public async Task<IActionResult> DeleteFunction(string id)
        {
            try
            {
                var response = await _functionService.DeleteFunction(id);
                if (response.StatusCode == StatusCodes.Status404NotFound)
                {
                    return NotFound(new
                    {
                        message = response.Message
                    });
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }

        #endregion

        #region Permission actions

        [AllowAnonymous]
        [HttpGet("permissions-by-role/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole([FromRoute(Name = "roleId")] int requestRoleId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return Unauthorized(new
                    {
                        message = "You need to be authenticated to access this resource."
                    });
                }

                // Lấy role từ token
                var currentRoleNames = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? "";
                var responseInfo = await _permissionService.GetListOfPermissionsByRole(currentRoleNames, requestRoleId);
                if (responseInfo.StatusCode == StatusCodes.Status404NotFound)
                {
                    return NotFound(new
                    {
                        message = responseInfo.Message
                    });
                }

                if (responseInfo.StatusCode == StatusCodes.Status403Forbidden)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        message = responseInfo.Message
                    });
                }

                return Ok(new
                {
                    screens = responseInfo.Data.TryGetValue("result", out var screens) ? screens : new { },
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }


        [HttpPost("permissions/update")]
        public async Task<IActionResult> UpdatePermission(PermissionStatus permission)
        {
            try
            {
                var response = await _permissionService.UpdatePermission(permission);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.InnerException?.Message ?? e.Message
                });
            }
        }
        #endregion
    }
}