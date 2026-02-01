using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetLikedSongList;

public record GetLikedSongListQuery(Guid UserId) : IQuery<Result<SongListVm>>;