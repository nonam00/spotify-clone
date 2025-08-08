using Application.Files.Interfaces;
using MediatR;

namespace Application.Files.Commands.DeleteFile;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
{
    private readonly IStorageProvider _storage;

    public DeleteFileCommandHandler(IStorageProvider storage)
    {
        _storage = storage;
    }

    public async Task Handle(DeleteFileCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _storage.DeleteFile(request.FileName);
            
        if (!success)
        {
            throw new Exception("File this such file name doesn't exist");
        }
    }
}