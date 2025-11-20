namespace Application.Shared.Interfaces;

public interface IFileServiceClient
{
    Task DeleteAsync(string path, CancellationToken cancellationToken);
}