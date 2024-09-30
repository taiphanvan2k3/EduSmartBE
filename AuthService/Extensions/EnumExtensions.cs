using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AuthService.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            // Tìm ra member của enum
            var enumMember = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (enumMember == null)
            {
                return string.Empty;
            }

            // Nếu có sử dụng [Display(Name = "Tên hiển thị")] thì lấy tên hiển thị
            var displayNameAttribute = enumMember.GetCustomAttribute<DisplayAttribute>();
            return displayNameAttribute?.Name ?? enumValue.ToString();
        }
    }
}