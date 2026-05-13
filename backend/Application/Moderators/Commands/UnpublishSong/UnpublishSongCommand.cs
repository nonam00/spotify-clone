using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UnpublishSong;

public sealed record UnpublishSongCommand(Guid ModeratorId, Guid SongId) : ICommand<Result>;