using AuthService.Services.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthService.Services.Permission.Schemas.Screen;
using AuthService.Services.Permission.Schemas.Function;
using AuthService.Services.Permission.Schemas;
using AuthService.Binders;
using System.Security.Claims;
using AuthService.Commons.Helpers;

namespace AuthService.Controllers
{
    [Route("auth/api/permission")]
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

        /// <summary>
        /// Get list of screens
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns>List of screens</returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     200 - Successful retrieval of screens
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "screens": [
        ///             { "id": "1", "name": "Screen 1", Code: "screen_1" },
        ///             { "id": "2", "name": "Screen 2", Code: "screen_2" }
        ///         ]
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
        [ProducesResponseType(typeof(IEnumerable<ScreenDto>), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Create a new screen
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="screen">Contains screen creation information</param>
        /// <returns>Details of the created screen</returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     201 - Screen created successfully
        ///     400 - Invalid screen info
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="201">
        /// Success
        /// 
        ///     {
        ///         "id": "new_screen_id",
        ///         "name": "New Screen",
        ///         "code": "new_screen",
        ///         "order": 1,
        ///         "functions": []
        ///     }
        /// </response>
        /// <response code="400">
        /// Validation error
        /// 
        ///     {
        ///         "message": "Invalid screen info"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

        /// <summary>
        /// Update an existing screen
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">The ID of the screen to be updated</param>
        /// <param name="screen">Contains updated screen information</param>
        /// <returns>Details of the updated screen</returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     200 - Screen updated successfully
        ///     400 - Invalid screen info
        ///     404 - Screen not found
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success: The new data after updating
        /// 
        ///     {
        ///         "id": "new_screen_id",
        ///         "name": "New Screen",
        ///         "code": "new_screen",
        ///         "order": 1,
        ///         "functions": []
        ///     }
        /// </response>
        /// <response code="400">
        /// Validation error
        /// 
        ///     {
        ///         "message": "Invalid screen info"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not found
        /// 
        ///     {
        ///         "message": "Screen not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

        /// <summary>
        /// Delete a screen
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">The ID of the screen to be deleted</param>
        /// <returns></returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     200 - Screen deleted successfully
        ///     404 - Screen not found
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "message": "Screen deleted successfully"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not found
        /// 
        ///     {
        ///         "message": "Screen not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

        /// <summary>
        /// Get list of functions
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="pagingInfo">Contains pagination information</param>
        /// <returns>List of functions</returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     200 - Successful retrieval of functions
        ///     400 - Invalid paging info
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "functions": [
        ///             { "id": "1", "name": "Function 1" },
        ///             { "id": "2", "name": "Function 2" }
        ///         ]
        ///     }
        /// </response>
        /// <response code="400">
        /// Validation error
        /// 
        ///     {
        ///         "message": "Invalid paging info"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

        /// <summary>
        /// Create a new function
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="function">Contains function creation information</param>
        /// <returns>Details of the created function</returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     201 - Function created successfully
        ///     400 - Invalid function info
        ///     404 - Not found
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="201">
        /// Success
        /// 
        ///     {
        ///         "id": "new_function_id",
        ///         "name": "New Function"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validation error
        /// 
        ///     {
        ///         "message": "Invalid function info"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not found
        /// 
        ///     {
        ///         "message": "Not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

        /// <summary>
        /// Update an existing function
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">The ID of the function to be updated</param>
        /// <param name="function">Contains updated function information</param>
        /// <returns>Details of the updated function</returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     200 - Function updated successfully
        ///     400 - Invalid function info
        ///     404 - Function not found
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "id": "function_id",
        ///         "name": "Updated Function"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validation error
        /// 
        ///     {
        ///         "message": "Invalid function info"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not found
        /// 
        ///     {
        ///         "message": "Function not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

        /// <summary>
        /// Delete a function
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">The ID of the function to be deleted</param>
        /// <returns></returns>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        /// Code
        /// 
        ///     200 - Function deleted successfully
        ///     404 - Function not found
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "message": "Function deleted successfully"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not found
        /// 
        ///     {
        ///         "message": "Function not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// 
        ///     {
        ///         "message": "Error message explaining the internal server issue"
        ///     }
        /// </response>
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

                return Ok(new
                {
                    id,
                    message = response.Message
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

        #endregion

        #region Permission actions

        /// <summary>
        /// Retrieve permissions associated with a specific role
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="requestRoleId">The ID of the role to retrieve permissions for</param>
        /// <returns>
        /// A list of permissions associated with the specified role.
        /// </returns>
        /// <remarks>
        /// NOTE:
        /// 
        ///     This API is used for any role to retrieve their permissions
        /// Code
        /// 
        ///     200 - Successfully retrieved permissions
        ///     401 - User needs to be authenticated
        ///     403 - User does not have permission to access this resource
        ///     404 - Role not found
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "screens": { /* List of screens/permissions */ }
        ///     }
        /// </response>
        /// <response code="401">
        /// Unauthorized
        /// 
        ///     {
        ///         "message": "You need to be authenticated to access this resource."
        ///     }
        /// </response>
        /// <response code="403">
        /// Forbidden
        /// 
        ///     {
        ///         "message": "Error message explaining why access is forbidden"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not Found
        /// 
        ///     {
        ///         "message": "Error message explaining why the role was not found"
        ///     }
        /// </response>
        [ProducesResponseType(typeof(IEnumerable<ScreenWithPermission>), StatusCodes.Status200OK)]
        [AllowAnonymous] // Ghi đè lại quyền truy cập cấp controller
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

        /// <summary>
        /// Update the status of a permission
        /// <para>Created at: 2024/09/10</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="permission">The permission status to update</param>
        /// <returns>
        /// A response indicating the result of the update operation.
        /// </returns>
        /// <remarks>
        /// NOTE:
        /// 
        ///     This API is used for any role to retrieve their permissions
        /// Code
        /// 
        ///     200 - Permission updated successfully
        ///     400 - Bad request for invalid permission data
        ///     500 - Internal server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "message": "OK"
        ///     }
        /// </response>
        /// <response code="400">
        /// Bad Request
        /// 
        ///     {
        ///         "message": "Permission is already active or inactive | You can't disable a permission that doesn't exist"
        ///     }
        /// </response>
        /// <response code="404">
        /// Not Found
        /// 
        ///     {
        ///         "message": "Role not found | You can't disable a permission that doesn't exist"
        ///     }
        /// </response>
        /// <response code="500">
        /// Internal Server Error
        /// 
        ///     {
        ///         "message": "Error message explaining what went wrong"
        ///     }
        /// </response>
        [HttpPut("permissions/update")]
        public async Task<IActionResult> UpdatePermission(PermissionStatus permission)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(
                    ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()));
                }

                var response = await _permissionService.UpdatePermission(permission);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new
                    {
                        message = response.Message,
                        permission = response.Data.TryGetValue("result", out var result) ? result : new { }
                    });
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    response.StatusCode, response.Error, response.Message));
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