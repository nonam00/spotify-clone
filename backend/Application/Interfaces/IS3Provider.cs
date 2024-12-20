using System.Net;

namespace Application.Interfaces
{
    public interface IS3Provider
    {
        Task<HttpStatusCode> UploadFile(Stream fileStream, string key, string contentType);
        Task<HttpStatusCode> DeleteFile(string key);
    }
}
