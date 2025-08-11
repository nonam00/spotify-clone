using MediatR;

using Application.Files.Interfaces;

namespace Application.Files.Queries.GetFile;

public class GetFileQueryHandler : IRequestHandler<GetFileQuery, Stream>
{
    private readonly IStorageProvider _storageProvider;

    public GetFileQueryHandler(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<Stream> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        var stream = await _storageProvider.GetFile(request.Name, request.MediaType, cancellationToken);
        return stream;
    }
}