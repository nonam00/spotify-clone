namespace Infrastructure.Minio;

public class MinioOptions
{
    public string Endpoint { get; init; } = null!;
    public string AccessKey { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
}