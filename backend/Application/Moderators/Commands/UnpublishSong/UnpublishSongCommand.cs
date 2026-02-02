using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UnpublishSong;

public record UnpublishSongCommand(Guid ModeratorId, Guid SongId) : ICommand<Result>;