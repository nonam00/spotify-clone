using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.AddSongsToPlaylist;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class AddSongsToPlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldAddSongToPlaylist_WhenValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        var song1 = Song.Create(
            "Test Song 1",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        var song2 = Song.Create(
            "Test Song 2",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        
        song1.Publish();
        song2.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddRangeAsync(song1, song2);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, playlist.Id, [song1.Id, song2.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var playlistSongs = await Context.Set<PlaylistSong>()
            .Where(ps => ps.PlaylistId == playlist.Id)
            .ToListAsync(CancellationToken.None);
        
        playlistSongs.Count.Should().Be(2);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongAlreadyInPlaylist()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        
        var song1 = Song.Create(
            "Test Song 1",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        var song2 = Song.Create(
            "Test Song 2",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        
        song1.Publish();
        song2.Publish();
        
        playlist.AddSong(song1.Id);
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddRangeAsync(song1, song2);
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, playlist.Id, [song1.Id, song2.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.SongAlreadyInPlaylist);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, Guid.NewGuid(), [song.Id]);

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
        var owner = User.Create(
            new Email("owner@example.com"),
            new PasswordHash("hashed_password"),
            "Owner");
        owner.Activate();
        
        var otherUser = User.Create(
            new Email("other@example.com"),
            new PasswordHash("hashed_password"),
            "Other User");
        otherUser.Activate();
        
        var playlist = Playlist.Create(owner.Id, "My Playlist");
        
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        
        song.Publish();
        
        await Context.Users.AddAsync(owner);
        await Context.Users.AddAsync(otherUser);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(otherUser.Id, playlist.Id, [song.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.OwnershipError);
    }
}