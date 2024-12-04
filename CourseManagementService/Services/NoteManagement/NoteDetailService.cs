using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.NoteManagement.Schemas;
using Microsoft.EntityFrameworkCore;
using TblNote = CourseManagementService.Database.Schemas.Note;

namespace CourseManagementService.Services.NoteManagement
{
    public interface INoteDetailService
    {
        /// <summary>
        /// Create a note in a lesson
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> CreateNote(NoteCreateDto noteCreateDto);

        /// <summary>
        /// Update a note
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> UpdateNote(NoteUpdateDto noteUpdateDto);

        /// <summary>
        /// Set a note as deleted
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of note</param>
        /// <returns></returns>
        public Task<ResponseInfo> SoftDeleteNote(Guid id);

        /// <summary>
        /// Delete a note permanently
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of note</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteNotePermanently(Guid id);
    }

    public class NoteDetailService(IServiceProvider serviceProvider, ILogger<NoteDetailService> logger)
        : BaseService(serviceProvider, logger), INoteDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new ArgumentNullException(ServiceInjectionError(nameof(IMapper)));

        public async Task<ResponseInfo> CreateNote(NoteCreateDto noteCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var lessonPermissionResponse = await CheckCanAccessLesson(noteCreateDto.LessonId, currentUser.UserId);
                if (!lessonPermissionResponse.IsSuccess)
                {
                    return lessonPermissionResponse;
                }

                var noteEntity = _mapper.Map<TblNote>(noteCreateDto);
                noteEntity.UserId = currentUser.UserId;
                noteEntity.LessonType = lessonPermissionResponse.Data["LessonType"];
                noteEntity.ChapterId = lessonPermissionResponse.Data["ChapterId"];

                await _context.Notes.AddAsync(noteEntity);
                await _context.SaveChangesAsync();

                return new ResponseInfo(resource: "id", noteEntity.Id);
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

        public async Task<ResponseInfo> UpdateNote(NoteUpdateDto noteUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canCRUDNoteResponse = await CheckCanCRUDNote(noteUpdateDto.Id, currentUser.UserId);
                if (!canCRUDNoteResponse.IsSuccess)
                {
                    return canCRUDNoteResponse;
                }

                TblNote noteEntity = canCRUDNoteResponse.Data["note"];
                noteEntity.Comment = noteUpdateDto.Comment;
                await _context.SaveChangesAsync();

                var noteDto = _mapper.Map<NoteDetail>(noteEntity);
                return new ResponseInfo(resource: "note", noteDto);
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

        public async Task<ResponseInfo> SoftDeleteNote(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canCRUDNoteResponse = await CheckCanCRUDNote(id, currentUser.UserId);
                if (!canCRUDNoteResponse.IsSuccess)
                {
                    return canCRUDNoteResponse;
                }

                TblNote noteEntity = canCRUDNoteResponse.Data["note"];
                noteEntity.IsDelFlag = true;
                await _context.SaveChangesAsync();

                return new ResponseInfo(resource: "id", id);
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

        public async Task<ResponseInfo> DeleteNotePermanently(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canCRUDNoteResponse = await CheckCanCRUDNote(id, currentUser.UserId);
                if (!canCRUDNoteResponse.IsSuccess)
                {
                    return canCRUDNoteResponse;
                }

                TblNote noteEntity = canCRUDNoteResponse.Data["note"];
                _context.Notes.Remove(noteEntity);
                await _context.SaveChangesAsync();

                return new ResponseInfo(resource: "id", id);
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

        private async Task<ResponseInfo> CheckCanCRUDNote(Guid noteId, int userId)
        {
            var noteEntity = await _context.Notes
                .Where(n => n.Id == noteId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (noteEntity == null)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Note not found");
            }

            if (noteEntity.UserId != userId)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You don't have permission to access this note");
            }

            var lessonPermissionResponse = await CheckCanAccessLesson(noteEntity.LessonId, userId);
            if (!lessonPermissionResponse.IsSuccess)
            {
                return lessonPermissionResponse;
            }

            return new ResponseInfo(resource: "note", noteEntity);
        }

        private async Task<ResponseInfo> CheckCanAccessLesson(Guid lessonId, int userId)
        {
            var lesson = await _context.Lessons
                .Where(l => l.Id == lessonId)
                .Select(l => new
                {
                    l.Chapter.Course.TeacherId,
                    IsEnrolled = l.Chapter.Course
                        .Enrollments.Any(e => e.StudentId == userId && !e.LeaveDate.HasValue),
                    l.LessonType,
                    l.ChapterId
                })
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
            }

            if (lesson.TeacherId != userId && !lesson.IsEnrolled)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You don't have permission to access this lesson");
            }

            var responseInfo = new ResponseInfo();
            responseInfo.Data.Add("LessonType", lesson.LessonType);
            responseInfo.Data.Add("ChapterId", lesson.ChapterId);

            return responseInfo;
        }
    }
}