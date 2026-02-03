using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.AddSongsToPlaylist;
using Application.Playlists.Errors;
using Application.Songs.Errors;
using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class AddSongsToPlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldAddSongToPlaylist_WhenValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author 2", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([song1, song2]);
        
        await Context.Users.AddAsync(user);
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
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author 2", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;

        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([song1, song2]);
        
        playlist.AddSong(song1);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, playlist.Id, [song1.Id, song2.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistDomainErrors.AlreadyContainsSong);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, Guid.NewGuid(), [Guid.NewGuid()]);

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
        
        var song = otherUser.UploadSong("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);        
        
        await Context.Users.AddRangeAsync(owner, otherUser);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(otherUser.Id, playlist.Id, [song.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.OwnershipError);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAddingUnpublishedSong()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author 2", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song1); // Song 2 remains unpublished
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, playlist.Id, [song1.Id, song2.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong);
    }
    
    [Fact] 
    public async Task Handle_ShouldReturnFailure_WhenSomeSongsNotFound()
    {
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());

        var user = User.Create(new Email("user@emal.com"), new PasswordHash("hashed_password"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        moderator.PublishSong(song1);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongsToPlaylistCommand(user.Id, playlist.Id, [song1.Id, Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongErrors.SongsNotFound);
    }
}