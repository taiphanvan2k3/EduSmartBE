namespace CourseManagementService.Common.Helpers
{
    public class EnumHelper
    {
        public static int ConvertEnumToInt(Enum value)
        {
            return (int)Enum.Parse(value.GetType(), value.ToString());
        }

        public static string GetEnumValue<T>(T value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}