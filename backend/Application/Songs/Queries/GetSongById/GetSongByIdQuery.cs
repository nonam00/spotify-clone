using Application.Shared.Messaging;

using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongById;

public class GetSongByIdQuery : IQuery<SongVm>
{
    public Guid SongId { get; init; }
}