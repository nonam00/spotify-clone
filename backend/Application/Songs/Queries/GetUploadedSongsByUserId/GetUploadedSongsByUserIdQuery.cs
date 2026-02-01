using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetUploadedSongsByUserId;

public record GetUploadedSongsByUserIdQuery(Guid UserId) : IQuery<Result<SongListVm>>;

