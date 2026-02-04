using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.PublishSong;

public record PublishSongCommand(Guid ModeratorId, Guid SongId) : ICommand<Result>;