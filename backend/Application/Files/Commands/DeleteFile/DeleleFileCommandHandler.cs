using MediatR;
using System.Net;

using Application.Interfaces;

namespace Application.Files.Commands.DeleteFile
{
    public class DeleteFileCommandHandler(IS3Provider s3)
        : IRequestHandler<DeleteFileCommand>
    {
        private readonly IS3Provider _s3 = s3;

        public async Task Handle(DeleteFileCommand request,
            CancellationToken cancellationToken)
        {
            var responseCode = await _s3.DeleteFile(request.FileName);

            if (responseCode != HttpStatusCode.NoContent)
            {
                throw new Exception("File this such file name doesn't exist");
            }
        }
    }
}
