using MediatR;

namespace Application.Files.Commands.DeleteFile;

public class DeleteFileCommand : IRequest
{
    public string FileName { get; init; } = null!;
}