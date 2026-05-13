using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeleteSong;

public sealed record DeleteSongCommand(Guid ModeratorId, Guid SongId) : ICommand<Result>;