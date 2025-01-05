namespace Infrastructure.Aws;

public class AwsOptions
{
    public string AccessKeyId { get; init; } = null!;
    public string SecretAccessKey { get; init; } = null!;
    public string ServiceURL { get; init; } = null!;
    public string BucketName { get; init; } = null!;
}