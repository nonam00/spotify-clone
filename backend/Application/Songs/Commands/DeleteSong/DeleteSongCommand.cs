using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.DeleteSong;

public record DeleteSongCommand(Guid Id) : ICommand<Result>;