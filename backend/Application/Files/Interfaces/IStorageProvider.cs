using Application.Files.Enums;

namespace Application.Files.Interfaces;

public interface IStorageProvider
{
    Task<string> UploadFile(Stream fileStream, MediaType mediaType, CancellationToken cancellationToken = default);
    Task DeleteFile(string name, MediaType mediaType, CancellationToken cancellationToken = default);
    Task<Stream> GetFile(string name, MediaType mediaType, CancellationToken cancellationToken = default);
}