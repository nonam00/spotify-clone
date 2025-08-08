using MediatR;

using Application.Files.Interfaces;

namespace Application.Files.Commands.UploadFile
{
    public class UploadFileCommandHandler(IStorageProvider storage)
        : IRequestHandler<UploadFileCommand, string>
    {
        private readonly IStorageProvider _storage = storage;

        public async Task<string> Handle(UploadFileCommand request,
            CancellationToken cancellationToken)
        {
            var fileName = Guid.NewGuid().ToString();
            var success = await _storage.UploadFile
            (
                request.FileStream,
                $"{request.ContentType}/{fileName}",
                request.ContentType + "/*"
            );

            if (!success)
            {
                throw new Exception("Failed on uploading file to the store");
            }
            
            return fileName;
        }
    }
}

