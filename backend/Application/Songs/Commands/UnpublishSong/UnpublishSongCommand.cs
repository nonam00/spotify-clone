using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.UnpublishSong;

public record UnpublishSongCommand(Guid Id) : ICommand<Result>;
