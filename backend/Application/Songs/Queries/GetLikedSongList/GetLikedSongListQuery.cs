using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongList;

public record GetLikedSongListQuery(Guid UserId) : IQuery<SongListVm>;