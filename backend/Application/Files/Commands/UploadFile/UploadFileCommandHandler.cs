using MediatR;

using Application.Files.Interfaces;

namespace Application.Files.Commands.UploadFile;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, string>
{
    private readonly IStorageProvider _storageProvider;

    public UploadFileCommandHandler(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var name = await _storageProvider.UploadFile(request.FileStream, request.MediaType, cancellationToken);
        return name;
    }
}