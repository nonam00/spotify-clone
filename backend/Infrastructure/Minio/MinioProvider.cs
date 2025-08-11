using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Minio;
using Minio.DataModel.Args;

using Application.Files.Enums;
using Application.Files.Interfaces;

namespace Infrastructure.Minio;

public class MinioProvider : IStorageProvider
{
    private readonly IMinioClient _minio;
    private readonly IMemoryCache _cache;

    public MinioProvider(IOptions<MinioOptions> options, IMemoryCache cache)
    {
        _minio = new MinioClient()
            .WithEndpoint(options.Value.Endpoint)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
            .WithSSL(false)
            .Build();
        _cache = cache;
    }

    public async Task<string> UploadFile(Stream fileStream, MediaType mediaType, CancellationToken cancellationToken)
    {
        var fileName = Guid.NewGuid().ToString();
        var contentType = mediaType.ToString().ToLower();
        
        var response = await _minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(contentType)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType + "/*"),
            cancellationToken);

        if ((int)response.ResponseStatusCode >= 300)
        {
            throw new Exception("Error on uploading file to S3:\n" + response.ResponseContent);
        }

        return fileName;
    }

    public async Task DeleteFile(string name, MediaType mediaType, CancellationToken cancellationToken)
    {
        var bucket = mediaType.ToString().ToLower();
        try
        {
            await _minio.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(name),
                cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error on deleting file from S3\n: {ex.Message}", ex);
        }
    }

    public async Task<Stream> GetFile(string name, MediaType mediaType, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(name, out byte[]? cachedStream) && cachedStream is not null)
        {
            return new MemoryStream(cachedStream);
        }
        
        var bucket = mediaType.ToString().ToLower();
        var stream = new MemoryStream();
        
        await _minio.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(name)
                .WithCallbackStream(data => data.CopyTo(stream)),
            cancellationToken);
        
        stream.Position = 0;

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(mediaType == MediaType.Image? 60 : 1));
        _cache.Set(name, stream.ToArray(), cacheOptions);
        
        return stream;
    }
}