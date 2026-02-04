using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeleteSongs;

public record DeleteSongsCommand(Guid ModeratorId, List<Guid> SongIds) : ICommand<Result>;