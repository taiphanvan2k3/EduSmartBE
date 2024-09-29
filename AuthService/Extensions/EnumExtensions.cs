using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AuthService.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumMember = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (enumMember == null)
            {
                return string.Empty;
            }

            var displayNameAttribute = enumMember.GetCustomAttribute<DisplayAttribute>();
            return displayNameAttribute?.Name ?? enumValue.ToString();
        }
    }
}