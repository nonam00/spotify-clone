namespace Application.Shared.Interfaces;

public interface IFileServiceClient
{
    Task DeleteAsync(string id, string bucket, CancellationToken cancellationToken);
}