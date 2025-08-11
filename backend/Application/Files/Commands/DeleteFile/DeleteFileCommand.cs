using MediatR;

using Application.Files.Enums;

namespace Application.Files.Commands.DeleteFile;

public class DeleteFileCommand : IRequest
{
    public string Name { get; init; } = null!;
    public MediaType MediaType { get; init; }
}