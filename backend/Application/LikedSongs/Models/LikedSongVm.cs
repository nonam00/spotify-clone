namespace Application.LikedSongs.Models;

public record LikedSongVm(Guid Id, string Title, string Author, string SongPath, string ImagePath);