using Application.Shared.Messaging;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public record DeleteLikedSongCommand(Guid UserId, Guid SongId) : ICommand;