using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseManagementService.Binders
{
    public class EnumBinder<TEnum> : IModelBinder where TEnum : struct, Enum
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<Enum>();

            foreach (var enumValue in enumValues)
            {
                var enumMember = enumValue.GetType()
                    .GetField(enumValue.ToString())
                    .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                    .FirstOrDefault() as EnumMemberAttribute;
                var enumName = enumValue.GetType().GetField(enumValue.ToString()).Name;

                // Nếu như dùng EnumMemberAttribute thì so sánh giá trị của EnumMemberAttribute với value
                if (enumMember != null && enumMember.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    bindingContext.Result = ModelBindingResult.Success(enumValue);
                    return Task.CompletedTask;
                }

                // Nếu không dùng EnumMemberAttribute thì so sánh tên enum với value
                if (enumName.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    bindingContext.Result = ModelBindingResult.Success(enumValue);
                    return Task.CompletedTask;
                }
            }

            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}