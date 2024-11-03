using System.Text;
using Xabe.FFmpeg;

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

        public static async Task<string> SaveFileLocally(string folderPath, string fileName, IFormFile file)
        {
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return filePath;
        }

        public static string ExtractPublicId(string url)
        {
            try
            {
                // Example http://res.cloudinary.com/da1aqhx1g/image/upload/v1729529214/q0joipmzjgfxdkugh9tj.png
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                var fileName = segments.Last(); // e.g., "q0joipmzjgfxdkugh9tj.png"
                var publicId = Path.GetFileNameWithoutExtension(fileName); // "q0joipmzjgfxdkugh9tj"
                return publicId;
            }
            catch
            {
                return "";
            }
        }

        public static async Task<int> GetDurationOfVideo(string filePath)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(filePath);
            var totalSeconds = mediaInfo.Duration.TotalSeconds;
            return (int)totalSeconds;
        }
    }
}