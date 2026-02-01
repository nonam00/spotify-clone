using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.UploadSong;

public record UploadSongCommand(
    Guid UserId,
    string Title,
    string Author,
    string SongPath,
    string ImagePath)
    : ICommand<Result>;