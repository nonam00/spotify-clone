using MediatR;

namespace Application.Files.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<string>
    {
        public Stream FileStream { get; init; } = null!;
        public string ContentType { get; init; } = null!;
    }
}
