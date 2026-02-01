using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Songs.Commands.PublishSong;

public record PublishSongCommand(Guid Id) : ICommand<Result>;
