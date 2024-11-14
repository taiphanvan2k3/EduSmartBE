using CourseManagementService.Services.AppState.Schemas;
using CourseManagementService.Services.CourseManagement.Public;
using CourseManagementService.Services.TagManagement.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.TagManagement
{
    public interface IListOfTagService
    {
        /// <summary>
        /// Get tags by keyword
        /// <para>Created at: 2024/11/14</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <returns></returns>
        Task<List<TagDto>> GetTags();
    }

    public class ListOfTagService(IServiceProvider serviceProvider,
        ILogger<ListOfPublicCourseService> logger) : BaseService(serviceProvider, logger), IListOfTagService
    {
        public async Task<List<TagDto>> GetTags()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                UserInfoState currentUser = GetCurrentUser();
                var currentUserId = currentUser?.UserId ?? 0;
                var listOfTags = await _context.Tags
                    .Where(x => x.IsCreatedByAdmin || x.CreatedBy == currentUserId)
                    .Select(x => new TagDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).ToListAsync();

                LogInfo("End", method);
                return listOfTags;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}