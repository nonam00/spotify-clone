using Application.Shared.Messaging;

namespace Application.LikedSongs.Commands.CreateLikedSong;

public record CreateLikedSongCommand(Guid UserId, Guid SongId) : ICommand<string>;
