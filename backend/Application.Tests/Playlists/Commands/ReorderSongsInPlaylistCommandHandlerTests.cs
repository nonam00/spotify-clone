using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.ReorderSongsInPlaylist;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class ReorderSongsInPlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReorderSongs_WhenValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        var song1 = Song.Create("Song 1", new FilePath("song1.mp3"), new FilePath("img1.jpg"), "Author");
        var song2 = Song.Create("Song 2", new FilePath("song2.mp3"), new FilePath("img2.jpg"), "Author");
        var song3 = Song.Create("Song 3", new FilePath("song3.mp3"), new FilePath("img3.jpg"), "Author");
        
        song1.Publish();
        song2.Publish();
        song3.Publish();
        
        playlist.AddSong(song1.Id);
        playlist.AddSong(song2.Id);
        playlist.AddSong(song3.Id);
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddRangeAsync(song1, song2, song3);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        // Reorder: song3, song1, song2
        var command = new ReorderSongsInPlaylistCommand(playlist.Id, [song3.Id, song1.Id, song2.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var playlistSongs = await Context.Set<PlaylistSong>()
            .Where(ps => ps.PlaylistId == playlist.Id)
            .OrderBy(ps => ps.Order)
            .ToListAsync();
        
        playlistSongs[0].SongId.Should().Be(song3.Id);
        playlistSongs[1].SongId.Should().Be(song1.Id);
        playlistSongs[2].SongId.Should().Be(song2.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var command = new ReorderSongsInPlaylistCommand(Guid.NewGuid(), [Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenPassingEmptyList()
    {
        // Arrange
        var command = new ReorderSongsInPlaylistCommand(Guid.NewGuid(), []);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
    }
}
