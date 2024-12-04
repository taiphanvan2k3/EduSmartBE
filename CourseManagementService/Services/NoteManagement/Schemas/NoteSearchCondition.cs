using CourseManagementService.Common.Schemas;
using CourseManagementService.Enumerations;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Services.NoteManagement.Schemas
{
    public class NoteSearchCondition : ParamsSearch
    {
        [FromQuery(Name = "type")]
        public NoteSearchType SearchType { get; set; }

        [FromQuery(Name = "sort")]
        public DateOrder SortOrder { get; set; }

        /// <summary>
        /// Id of lesson or chapter or course
        /// </summary>
        [FromQuery(Name = "resourceId")]
        public Guid ResourceId { get; set; }
    }
}