using CourseManagementService.Common;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;

namespace CourseManagementService.Services.LessonManagement.TextLesson
{
    public interface ITextLessonDetailService
    {
        /// <summary>
        /// Get text lesson detail
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para> 
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetTextLessonDetail(Guid lessonId);

        /// <summary>
        /// Create text lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CreateTextLesson(TextLessonCreateUpdateDto TextLessonCreateDto);

        /// <summary>
        /// Update text lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para>
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <param name="textLessonUpdateDto">Data to update</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateTextLesson(Guid lessonId, TextLessonCreateUpdateDto textLessonUpdateDto);

        /// <summary>
        /// Delete text lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteTextLesson(Guid lessonId);
    }

    public class TextLessonDetailService : ITextLessonDetailService
    {
        public Task<ResponseInfo> GetTextLessonDetail(Guid lessonId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseInfo> CreateTextLesson(TextLessonCreateUpdateDto TextLessonCreateDto)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseInfo> UpdateTextLesson(Guid lessonId, TextLessonCreateUpdateDto textLessonUpdateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseInfo> DeleteTextLesson(Guid lessonId)
        {
            throw new NotImplementedException();
        }
    }
}