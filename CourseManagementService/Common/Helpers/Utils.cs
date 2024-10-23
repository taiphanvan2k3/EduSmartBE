using System.Text;

namespace CourseManagementService.Common.Helpers
{
    public class Utils
    {
        public static string ConvertStringToBase64(string input)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}