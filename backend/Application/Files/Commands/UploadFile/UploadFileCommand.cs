using MediatR;

namespace Application.Files.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<string>
    {
        public Stream FileStream { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
