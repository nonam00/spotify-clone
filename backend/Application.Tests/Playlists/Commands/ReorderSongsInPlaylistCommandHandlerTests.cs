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

        var playlist = user.CreatePlaylist().Value;
        
        var song1 = Song.Create("Song 1", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg"));
        var song2 = Song.Create("Song 2", "Author", new FilePath("song2.mp3"), new FilePath("img2.jpg"));
        var song3 = Song.Create("Song 3", "Author", new FilePath("song3.mp3"), new FilePath("img3.jpg"));
        
        song1.Publish();
        song2.Publish();
        song3.Publish();

        playlist.AddSongs([song1, song2, song3]);
        
        await Context.Songs.AddRangeAsync(song1, song2, song3);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Reorder: song3, song1, song2
        var command = new ReorderSongsInPlaylistCommand(user.Id, playlist.Id, [song3.Id, song1.Id, song2.Id]);

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
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new ReorderSongsInPlaylistCommand(user.Id, Guid.NewGuid(), [Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotOwner()
    {
        // Arrange
        var owner = User.Create(new Email("owner@example.com"), new PasswordHash("hashed_password"), "Owner");
        owner.Activate();
        
        var otherUser = User.Create(
            new Email("other@example.com"),
            new PasswordHash("hashed_password"),
            "Other User");
        otherUser.Activate();

        var playlist = owner.CreatePlaylist().Value;
        
        await Context.Users.AddRangeAsync(owner, otherUser);
        await Context.SaveChangesAsync();
        
        var command = new ReorderSongsInPlaylistCommand(otherUser.Id, playlist.Id, [Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.OwnershipError);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPassingListWithSongNotInPlaylist()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        
        var song1 = Song.Create("Song 1", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg"));
        var song2 = Song.Create("Song 2", "Author", new FilePath("song2.mp3"), new FilePath("img2.jpg"));
        var song3 = Song.Create("Song 3", "Author", new FilePath("song3.mp3"), new FilePath("img3.jpg"));
        
        song1.Publish();
        song2.Publish();
        song3.Publish();

        playlist.AddSongs([song1, song2]);
        
        await Context.Songs.AddRangeAsync(song1, song2, song3);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Reorder: song3, song1, song2
        var command = new ReorderSongsInPlaylistCommand(user.Id, playlist.Id, [song3.Id, song1.Id, song2.Id]);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistDomainErrors.DoesntContainSong);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenPassingEmptyList()
    {
        // Arrange
        var command = new ReorderSongsInPlaylistCommand(Guid.NewGuid(),Guid.NewGuid(), []);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
    }
}
