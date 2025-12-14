using Application.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Files;

public class FileServiceClient :  IFileServiceClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://nginx/files/api/v1";
    private readonly ILogger<FileServiceClient> _logger;
    private readonly string _apiKey;
    
    public FileServiceClient(ILogger<FileServiceClient> logger, IOptions<FileServiceOptions> options)
    {
        _logger = logger;
        _apiKey = options.Value.ApiKey;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending request to delete file {path}", path);
        
        var request = new HttpRequestMessage(HttpMethod.Delete, "?type=image&file_id=" + path);
        
        // Добавляем API ключ в заголовок, если он настроен
        if (!string.IsNullOrEmpty(_apiKey))
        {
            request.Headers.Add("X-API-Key", _apiKey);
        }
        
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}