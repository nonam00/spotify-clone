using MediatR;
using System.Net;

using Application.Interfaces;

namespace Application.Files.Commands.UploadFile
{
    public class UploadFileCommandHandler(IS3Provider s3)
        : IRequestHandler<UploadFileCommand, string>
    {
        private readonly IS3Provider _s3 = s3;

        public async Task<string> Handle(UploadFileCommand request,
            CancellationToken cancellationToken)
        {
            string fileName = Guid.NewGuid().ToString();
            var responseCode = await _s3.UploadFile
            (
                request.FileStream,
                $"{request.ContentType}/{fileName}",
                request.ContentType + "/*"
            );

            if (responseCode != HttpStatusCode.OK)
            {
                throw new Exception("Failed on uploading file to the store");
            }
            
            return fileName;
        }
    }
}

