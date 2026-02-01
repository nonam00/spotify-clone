using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Songs.Commands.PublishSongs;

public record PublishSongsCommand(List<Guid> SongIds) : ICommand<Result>;
