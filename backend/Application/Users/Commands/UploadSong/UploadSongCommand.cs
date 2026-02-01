using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.UploadSong;

public record UploadSongCommand(
    Guid UserId,
    string Title,
    string Author,
    string SongPath,
    string ImagePath)
    : ICommand<Result>;