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

        public static void CreateUploadFolderIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static async Task SaveFileLocally(string folderPath, string fileName, IFormFile file)
        {
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}