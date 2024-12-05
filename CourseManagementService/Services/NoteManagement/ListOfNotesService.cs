using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.Common;
using CourseManagementService.Enumerations;
using CourseManagementService.Extensions;
using CourseManagementService.Services.NoteManagement.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.NoteManagement
{
    public interface IListOfNotesService
    {
        /// <summary>
        /// Get notes of a course, chapter or lesson
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        public Task<ResponseInfo> GetMyNotes(NoteSearchCondition noteSearchCondition);
    }

    public class ListOfNotesService(IServiceProvider serviceProvider, ILogger<ListOfNotesService> logger)
        : BaseService(serviceProvider, logger), IListOfNotesService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));

        public async Task<ResponseInfo> GetMyNotes(NoteSearchCondition noteSearchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var courseInfo = await _context.Lessons
                    .Where(l => l.Id == noteSearchCondition.ResourceId
                        || l.ChapterId == noteSearchCondition.ResourceId
                        || l.Chapter.CourseId == noteSearchCondition.ResourceId)
                    .Select(l => new
                    {
                        l.Chapter.Course.TeacherId,
                        IsEnrolled = l.Chapter.Course.Enrollments.Any(e => e.StudentId == currentUser.UserId && !e.LeaveDate.HasValue)
                    })
                    .FirstOrDefaultAsync();

                if (courseInfo == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Resource not found");
                }

                if (courseInfo.TeacherId != currentUser.UserId && !courseInfo.IsEnrolled)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You are not allowed to access this resource");
                }

                var notesQuery = _context.Notes.Where(n => n.UserId == currentUser.UserId);
                notesQuery = noteSearchCondition.SearchType switch
                {
                    NoteSearchType.Course => notesQuery.Where(n => n.Chapter.CourseId == noteSearchCondition.ResourceId),
                    NoteSearchType.Chapter => notesQuery.Where(n => n.ChapterId == noteSearchCondition.ResourceId),
                    NoteSearchType.Lesson => notesQuery.Where(n => n.LessonId == noteSearchCondition.ResourceId),
                    _ => throw new InvalidOperationException("Invalid search type")
                };

                if (noteSearchCondition.SortOrder == DateOrder.Latest)
                {
                    notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
                }
                else
                {
                    notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
                }

                var notes = await notesQuery
                    .ProjectTo<NoteDetail>(_mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(noteSearchCondition.CurrentPage, noteSearchCondition.PageSize);

                return new ResponseInfo(resource: "notes", data: notes);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }
    }
}