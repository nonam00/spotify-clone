using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.PublishSongs;

public record PublishSongsCommand(List<Guid> SongIds) : ICommand<Result>;
