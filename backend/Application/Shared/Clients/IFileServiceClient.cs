namespace Application.Shared.Clients;

public interface IFileServiceClient
{
    Task DeleteAsync(string path, CancellationToken cancellationToken);
}