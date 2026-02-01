using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Songs.Commands.UnpublishSong;

public record UnpublishSongCommand(Guid Id) : ICommand<Result>;
