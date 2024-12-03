using System.ComponentModel.DataAnnotations;
using CourseManagementService.Binders;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class LessonRatingSearchCondition : ParamsSearch
    {
        [SwaggerIgnore]
        public Guid LessonId { get; set; }

        [FromQuery(Name = "filter")] // Dùng filter làm tên query string thay vì RatingFilter
        [ModelBinder(BinderType = typeof(EnumBinder<LessonRatingFilter>))] // Hỗ trợ binding enum từ query string khi dùng value không phải là tên enum
        [EnumDataType(typeof(LessonRatingFilter))] // ModelValidation sẽ kiểm tra giá trị của RatingFilter có thuộc enum LessonRatingFilter không
        public LessonRatingFilter RatingFilter { get; set; }
    }
}