namespace Infrastructure
{
    public class AwsOptions
    {
       public string AccessKeyId { get; set; } = null!;
       public string SecretAccessKey { get; set; } = null!;
       public string ServiceURL { get; set; } = null!;
       public string BucketName { get; set; } = null!;
    }
}
