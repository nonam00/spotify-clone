using Application.Shared.Messaging;

using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId;

public class GetSongListByPlaylistIdQuery : IQuery<SongListVm>
{
    public Guid PlaylistId { get; init; } 
}