using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Songs.Commands.DeleteSongs;

public record DeleteSongsCommand(List<Guid> SongIds) : ICommand<Result>;