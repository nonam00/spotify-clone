using MediatR;

using Application.Files.Enums;

namespace Application.Files.Queries.GetFile;

public class GetFileQuery : IRequest<Stream>
{
    public string Name { get; set; } = null!;
    public MediaType MediaType { get; set; }
}