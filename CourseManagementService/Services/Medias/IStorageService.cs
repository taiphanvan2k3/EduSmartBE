using System.IO;
using System.Threading.Tasks;

namespace CourseManagementService.Services.Medias
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream stream, string objectName, string contentType);
        Task<string> UploadFileFromLocalAsync(string localPath, string objectName, string contentType);
        Task<string> GetPresignedUrlAsync(string objectName, int expireTimeInMinutes = 60);
        Task DeleteFileAsync(string objectName);
    }
}
