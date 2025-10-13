using Application.Files.Interfaces;
using MediatR;

namespace Application.Files.Queries.GetPresignedUrl;

public class GetPresignedUrlQueryHandler : IRequestHandler<GetPresignedUrlQuery, string>
{
    private readonly IStorageProvider _storageProvider;

    public GetPresignedUrlQueryHandler(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<string> Handle(GetPresignedUrlQuery request, CancellationToken cancellationToken)
    {
        var url = await _storageProvider.GetPresignedUrl(request.Name, request.MediaType, cancellationToken);
        return url;
    }
}