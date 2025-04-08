using System.Net;

namespace Application.Interfaces
{
    public interface IStorageProvider
    {
        Task<bool> UploadFile(Stream fileStream, string key, string contentType);
        Task<bool> DeleteFile(string key);
    }
}
