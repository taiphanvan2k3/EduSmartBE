using CourseManagementService.Services.NoteManagement;
using CourseManagementService.Services.NoteManagement.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course-service/api", Order = 13)]
    [ApiController]
    [Authorize]
    public class NoteController(
        IListOfNotesService listOfNotesService,
        INoteDetailService noteDetailService) : BaseController
    {
        private readonly IListOfNotesService _listOfNotesService = listOfNotesService
            ?? throw new ArgumentNullException(nameof(listOfNotesService));
        private readonly INoteDetailService _noteDetailService = noteDetailService
            ?? throw new ArgumentNullException(nameof(noteDetailService));

        /// <summary>
        /// Get list of my notes
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpGet("notes")]
        public async Task<IActionResult> GetMyNotes([FromQuery] NoteSearchCondition noteSearchCondition)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var response = await _listOfNotesService.GetMyNotes(noteSearchCondition);
            return HandleResponseInfo(response, resourceName: "notes", isWrapperInObject: false);
        }

        /// <summary>
        /// Create a note in a lesson
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <param name="noteCreateDto">Note information</param>
        [HttpPost("lesson/{lessonId}/notes")]
        public async Task<IActionResult> CreateNote([FromRoute] Guid lessonId, [FromBody] NoteCreateDto noteCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            noteCreateDto.LessonId = lessonId;
            var response = await _noteDetailService.CreateNote(noteCreateDto);
            return HandleResponseInfo(response, resourceName: "id");
        }

        /// <summary>
        /// Update a note
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of note</param>
        /// <param name="noteUpdateDto">Note information to update</param>
        [HttpPut("notes/{id}")]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] NoteUpdateDto noteUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            noteUpdateDto.Id = id;
            var response = await _noteDetailService.UpdateNote(noteUpdateDto);
            return HandleResponseInfo(response, resourceName: "note");
        }

        /// <summary>
        /// Set a note as deleted
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of note</param>
        [HttpDelete("notes/{id}/soft-delete")]
        public async Task<IActionResult> DeleteNote([FromRoute] Guid id)
        {
            var response = await _noteDetailService.SoftDeleteNote(id);
            return HandleResponseInfo(response, resourceId: id.ToString());
        }

        /// <summary>
        /// Delete a note permanently
        /// <para>Created at: 2024/12/04</para>
        /// </summary>
        /// <param name="id">Id of note</param>
        [HttpDelete("notes/{id}")]
        public async Task<IActionResult> DeleteNotePermanently([FromRoute] Guid id)
        {
            var response = await _noteDetailService.DeleteNotePermanently(id);
            return HandleResponseInfo(response, resourceId: id.ToString());
        }
    }
}