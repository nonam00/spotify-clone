using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Songs.Commands.DeleteSong;

public record DeleteSongCommand(Guid Id) : ICommand<Result>;