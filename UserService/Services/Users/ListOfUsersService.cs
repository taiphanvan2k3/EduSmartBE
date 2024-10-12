using Microsoft.EntityFrameworkCore;
using UserService.Commons.Helpers;
using UserService.Commons.Schemas;
using UserService.Extensions;
using UserService.Services.Users.Schemas;

namespace UserService.Services.Users
{
    public interface IListOfUsersService
    {
        public Task<PaginatedList<UserDto>> GetUsers(SearchCondition searchCondition);
    }

    public class ListOfUsersService(IServiceProvider serviceProvider, ILogger<ListOfUsersService> logger)
        : BaseService(serviceProvider, logger), IListOfUsersService
    {
        public async Task<PaginatedList<UserDto>> GetUsers(SearchCondition searchCondition)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                searchCondition.Email = searchCondition.Email.Trim();
                searchCondition.Username = searchCondition.Username.Trim();
                searchCondition.FullName = searchCondition.FullName.Trim();

                var usersQuery = _context.Users
                    .Where(x => (string.IsNullOrEmpty(searchCondition.Email)
                            || EF.Functions.ILike(x.Email, $"%{searchCondition.Email}%"))
                        && (string.IsNullOrEmpty(searchCondition.Username)
                            || EF.Functions.ILike(x.UserName, $"%{searchCondition.Username}%"))
                        && (string.IsNullOrEmpty(searchCondition.FullName)
                            || EF.Functions.ILike(x.UserInfo.LastName + " " + x.UserInfo.FirstName, $"%{searchCondition.FullName}%")))
                    .Select(x => new UserDto()
                    {
                        Id = x.Id,
                        Username = x.UserName,
                        Email = x.Email,
                        FirstName = x.UserInfo.FirstName,
                        LastName = x.UserInfo.LastName,
                        AvatarURL = x.UserInfo.AvatarURL,
                        Gender = x.UserInfo.Gender,
                        GenderName = Utils.GetGenderName(x.UserInfo.Gender),
                        IsActive = x.IsActive,
                        Roles = x.UserRoles.Select(ur => ur.Role.Name).ToList(),
                        CreatedAt = x.CreatedAt
                    });

                var paginatedUsers = await usersQuery.ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                LogInfo("End", method);
                return paginatedUsers;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}