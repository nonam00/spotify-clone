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

    public async Task DeleteAsync(string id, string bucket, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending request to delete file {path} from bucket {bucket}", id, bucket);
        
        var request = new HttpRequestMessage(HttpMethod.Delete, $"?type={bucket}&file_id={id}");
        
        // Adding API Key to headers 
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