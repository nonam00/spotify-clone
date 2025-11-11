using Application.Shared.Clients;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Files;

public class FileServiceClient :  IFileServiceClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://nginx/files/api/v1";
    private readonly ILogger<FileServiceClient> _logger;
    
    public FileServiceClient(ILogger<FileServiceClient> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending request to delete file {path}", path);
        await _httpClient.DeleteAsync("?type=image&file_id=" + path, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}