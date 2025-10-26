using Application.Shared.Messaging;

using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetAllSongs;

public record GetAllSongsQuery : IQuery<SongListVm>;