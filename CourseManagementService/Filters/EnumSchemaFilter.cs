using System.Runtime.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CourseManagementService.Filters
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        // Convert về dạng ví dụ 1-Active, 2-Inactive
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum = Enum.GetValues(context.Type)
                    .Cast<object>()
                    .Select(e => new OpenApiString(GetEnumDescription(context.Type, e)))
                    .Cast<IOpenApiAny>() // Chuyển đổi đúng kiểu
                    .ToList();
            }
        }

        /// <summary>
        /// Dùng cái này nếu muốn dạng hiển thị sẽ dựa theo tên của Enum
        /// </summary>
        /// <returns></returns>
        private static string GetEnumName(Type enumType, object value)
        {
            return Enum.GetName(enumType, value);
        }

        /// <summary>
        /// Dùng cái này nếu muốn dạng hiển thị sẽ dựa theo EnumMemberAttribute của Enum
        /// </summary>
        /// <returns></returns>
        private static string GetEnumDescription(Type enumType, object value)
        {
            var field = enumType.GetField(value.ToString());
            var attribute = field?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            return attribute?.Value ?? value.ToString();
        }
    }
}