using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetUploadedSongsByUserId;

public record GetUploadedSongsByUserIdQuery(Guid UserId) : IQuery<Result<SongListVm>>;

