using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.PublishSong;

public record PublishSongCommand(Guid Id) : ICommand<Result>;
