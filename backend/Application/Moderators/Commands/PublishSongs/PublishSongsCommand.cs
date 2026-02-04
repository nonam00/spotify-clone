using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.PublishSongs;

public record PublishSongsCommand(Guid ModeratorId, List<Guid> SongIds) : ICommand<Result>;
