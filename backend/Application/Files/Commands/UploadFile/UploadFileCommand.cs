using MediatR;

using Application.Files.Enums;

namespace Application.Files.Commands.UploadFile;

public class UploadFileCommand : IRequest<string>
{
    public Stream FileStream { get; set; } = null!;
    public MediaType MediaType { get; set; }
}