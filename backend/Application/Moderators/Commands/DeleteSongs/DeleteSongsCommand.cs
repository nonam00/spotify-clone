using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeleteSongs;

public sealed record DeleteSongsCommand(Guid ModeratorId, List<Guid> SongIds) : ICommand<Result>;