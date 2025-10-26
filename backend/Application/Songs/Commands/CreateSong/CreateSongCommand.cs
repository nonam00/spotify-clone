using Application.Shared.Messaging;

namespace Application.Songs.Commands.CreateSong;

public record CreateSongCommand(
    Guid UserId, string Title, string Author, string SongPath, string ImagePath): ICommand<Guid>;