using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.DeleteSongs;

public record DeleteSongsCommand(List<Guid> SongIds) : ICommand<Result>;