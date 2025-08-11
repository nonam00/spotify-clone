using MediatR;

using Application.Files.Interfaces;

namespace Application.Files.Commands.DeleteFile;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
{
    private readonly IStorageProvider _storageProvider;

    public DeleteFileCommandHandler(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        await _storageProvider.DeleteFile(request.Name, request.MediaType, cancellationToken);
    }
}