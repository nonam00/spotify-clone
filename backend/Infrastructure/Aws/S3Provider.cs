using Amazon.S3;
using Amazon.S3.Model;
using Application.Files.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Aws
{
    public class S3Provider : IStorageProvider
    {
        private readonly AmazonS3Client _s3;
        private readonly AwsOptions _options;

        public S3Provider(IOptions<AwsOptions> options)
        {
            _options = options.Value;
            _s3 = new AmazonS3Client
            (
                _options.AccessKeyId,
                _options.SecretAccessKey,
                new AmazonS3Config
                {
                    RegionEndpoint = null,
                    ServiceURL = _options.ServiceURL 
                }
            );
        }

        public async Task<bool> UploadFile(Stream fileStream, string key, string contentType)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                ContentType = contentType,
                InputStream = fileStream 
            };

            var response = await _s3.PutObjectAsync(putObjectRequest);
            return (int)response.HttpStatusCode is >= 200 and < 300;
        }
         
        public async Task<bool> DeleteFile(string key)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            var response = await _s3.DeleteObjectAsync(deleteObjectRequest);
            return (int)response.HttpStatusCode is >= 200 and < 300;
        }
    }
}
