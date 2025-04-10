using MediatR;

using Application.Interfaces;

namespace Application.Files.Commands.DeleteFile
{
    public class DeleteFileCommandHandler(IStorageProvider storage)
        : IRequestHandler<DeleteFileCommand>
    {
        private readonly IStorageProvider _storage = storage;

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
}
