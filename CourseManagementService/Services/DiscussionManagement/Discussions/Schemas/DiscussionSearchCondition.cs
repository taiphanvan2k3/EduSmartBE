using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using CourseManagementService.Binders;
using CourseManagementService.Common.Schemas;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionSearchCondition : ParamsSearch
    {
        [SwaggerIgnore]
        public Guid CourseId { get; set; }

        [FromQuery(Name = "filter")]
        [ModelBinder(BinderType = typeof(EnumBinder<DiscussionFilter>))]
        [EnumDataType(typeof(DiscussionFilter))]
        public DiscussionFilter Filter { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiscussionFilter
    {
        [EnumMember(Value = "created-by-me")]
        CreatedByMe = 1,

        [EnumMember(Value = "reply-by-me")]
        ReplyByMe = 2,

        [EnumMember(Value = "for-teacher")]
        ForTeacher = 3
    }
}