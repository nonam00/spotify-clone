using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeleteSong;

public record DeleteSongCommand(Guid ModeratorId, Guid SongId) : ICommand<Result>;