using Application.Files.Enums;
using MediatR;

namespace Application.Files.Queries.GetPresignedUrl;

public class GetPresignedUrlQuery : IRequest<string>
{
    public string Name { get; set; } = null!;
    public MediaType MediaType { get; set; }
}