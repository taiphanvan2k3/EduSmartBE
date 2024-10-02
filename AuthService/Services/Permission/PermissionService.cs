using AuthService.Commons;
using AuthService.Services.Permission.Schemas;
using AuthService.Services.Permission.Schemas.Screen;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TblPermission = AuthService.Databases.Schemas.Permission;

namespace AuthService.Services.Permission
{
    public interface IPermissionService
    {
        /// <summary>
        /// Get list of permission by role id
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <param name="currentRoleNames">Current role names in token</param>
        /// <param name="requestRoleId">Role id from parameters to get permission</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetListOfPermissionsByRole(string currentRoleNames, int requestRoleId);

        /// <summary>
        /// Update permission status
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdatePermission(PermissionStatus permission);
    }

    public class PermissionService(IServiceProvider serviceProvider,
        RoleManager<IdentityRole<int>> roleManager, IMapper mapper, ILogger<PermissionService> logger)
        : BaseService(serviceProvider, logger), IPermissionService
    {
        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));

        private readonly RoleManager<IdentityRole<int>> _roleManager = roleManager
            ?? throw new ArgumentNullException(nameof(roleManager));

        public async Task<ResponseInfo> GetListOfPermissionsByRole(string currentRoleNames, int requestRoleId)
        {
            try
            {
                _logger.LogInformation("[PermissionService][GetListPermission] Start");
                var responseInfo = new ResponseInfo();

                var requestedRole = await _roleManager.FindByIdAsync(requestRoleId.ToString());
                if (requestedRole == null)
                {
                    responseInfo.Message = "Role not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    _logger.LogInformation("[PermissionService][GetListPermission] End");
                    return responseInfo;
                }

                // Chỉ có admin mới xem được tất cả các Permission của các Role khác
                // Còn lại chỉ xem được Permission của Role mà nó có
                if (!currentRoleNames.Contains("Admin") && !currentRoleNames.Contains(requestedRole.Name))
                {
                    responseInfo.Message = "You don't have permission to get permissions of this role";
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    _logger.LogInformation("[PermissionService][GetListPermission] End");
                    return responseInfo;
                }

                var permissionByRole = await _context.Permissions
                    .Where(x => x.RoleId == requestRoleId)
                    .ToDictionaryAsync(x => x.FunctionId, x => true);

                var result = await _context.Screens
                    .AsNoTracking()
                    .OrderBy(x => x.Order)
                    .Select(x => new ScreenWithPermission()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        Permissions = x.Functions
                            .OrderBy(f => f.Order)
                            .Select(f => new PermissionDto()
                            {
                                FunctionId = f.Id,
                                FunctionName = f.Name,
                                IsActive = permissionByRole.ContainsKey(f.Id)
                            })
                            .ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation("[PermissionService][GetListPermission] End");
                responseInfo.Data.Add("result", result);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[PermissionService][GetListPermission][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdatePermission(PermissionStatus permission)
        {
            try
            {
                _logger.LogInformation("[PermissionService][UpdatePermission] Start");
                var responseInfo = new ResponseInfo();

                var requestedRole = await _roleManager.FindByIdAsync(permission.RoleId.ToString());
                if (requestedRole == null)
                {
                    responseInfo.Message = "Role not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                var existedPermission = await _context.Permissions
                    .FirstOrDefaultAsync(x => x.RoleId == permission.RoleId && x.FunctionId == permission.FunctionId);

                if (existedPermission == null)
                {
                    if (!permission.IsActive)
                    {
                        responseInfo.Message = "You can't disable a permission that doesn't exist";
                        responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                        return responseInfo;
                    }
                    else
                    {
                        var newPermission = _mapper.Map<TblPermission>(permission);
                        await _context.Permissions.AddAsync(newPermission);

                        await _context.SaveChangesAsync();

                        responseInfo.Data.Add("result", newPermission);
                    }

                    _logger.LogInformation("[PermissionService][UpdatePermission] End");
                    return responseInfo;
                }

                if (permission.IsActive)
                {
                    responseInfo.Message = "Permission is already active";
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    _context.Permissions.Remove(existedPermission);
                    await _context.SaveChangesAsync();
                    responseInfo.Data.Add("result", new
                    {
                        message = "Permission is disabled"
                    });
                }

                _logger.LogInformation("[PermissionService][UpdatePermission] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PermissionService][UpdatePermission][{Error}]", e.Message);
                throw;
            }
        }
    }
}