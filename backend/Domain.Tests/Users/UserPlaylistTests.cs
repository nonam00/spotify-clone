using Domain.Errors;
using Domain.Events;
using Domain.Models;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests.Users;

public class UserPlaylistTests
{
    private readonly User _activeUser;
    private readonly User _inactiveUser;

    public UserPlaylistTests()
    {
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("TestPassword123!");
            
        _activeUser = User.Create(email, passwordHash);
        _activeUser.Activate();
            
        _inactiveUser = User.Create(email, passwordHash);
    }
        
    [Fact]
    public void Playlists_ShouldReturnEmptyCollectionInitially()
    {
        // Act
        var user = User.Create(new Email("email@example.com"), new PasswordHash("hashed_password"));

        // Assert
        user.Playlists.Should().BeEmpty();
    }

    [Fact]
    public void CreatePlaylist_WhenUserIsActive_ShouldCreatePlaylistSuccessfully()
    {
        // Act
        var result = _activeUser.CreatePlaylist();

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.Playlists.Should().ContainSingle();
            
        var playlist = result.Value;
        playlist.UserId.Should().Be(_activeUser.Id);
        playlist.Title.Should().StartWith("Playlist #");
        playlist.ImagePath.Value.Should().BeEmpty();
        playlist.PlaylistSongs.Should().BeEmpty();
    }

    [Fact]
    public void CreatePlaylist_ShouldIncrementPlaylistNumber()
    {
        // Act
        var result1 = _activeUser.CreatePlaylist();
        var result2 = _activeUser.CreatePlaylist();
        var result3 = _activeUser.CreatePlaylist();

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        result3.IsSuccess.Should().BeTrue();
            
        _activeUser.Playlists.Should().HaveCount(3);
        result1.Value.Title.Should().Be("Playlist #1");
        result2.Value.Title.Should().Be("Playlist #2");
        result3.Value.Title.Should().Be("Playlist #3");
    }

    [Fact]
    public void CreatePlaylist_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Act
        var result = _inactiveUser.CreatePlaylist();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
        _inactiveUser.Playlists.Should().BeEmpty();
    }

    [Fact]
    public void RemovePlaylist_WhenUserIsActiveAndPlaylistExists_ShouldRemoveSuccessfully()
    {
        // Arrange
        var playlistResult = _activeUser.CreatePlaylist();
        var playlist = playlistResult.Value;
        _activeUser.Playlists.Should().ContainSingle();

        // Act
        var result = _activeUser.RemovePlaylist(playlist.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.Playlists.Should().BeEmpty();
    }

    [Fact]
    public void RemovePlaylist_WhenPlaylistHasImage_ShouldRaisePlaylistDeletedEvent()
    {
        // Arrange
        var playlistResult = _activeUser.CreatePlaylist();
        var playlist = playlistResult.Value;
            
        // Add image to playlist
        var imagePath = new FilePath("playlist.jpg");
        playlist.UpdateDetails("Updated Title", "Description", imagePath);
            
        _activeUser.CleanDomainEvents();

        // Act
        var result = _activeUser.RemovePlaylist(playlist.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.DomainEvents.Should().ContainSingle(e => e is PlaylistDeletedEvent);
        var domainEvent = _activeUser.DomainEvents.OfType<PlaylistDeletedEvent>().First();
        domainEvent.PlaylistId.Should().Be(playlist.Id);
        domainEvent.ImagePath.Should().Be(imagePath);
    }

    [Fact]
    public void RemovePlaylist_WhenPlaylistDoesNotExist_ShouldReturnFailure()
    {
        // Act
        var result = _activeUser.RemovePlaylist(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.UserDoesNotHavePlaylist);
    }

    [Fact]
    public void RemovePlaylist_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Act
        var result = _inactiveUser.RemovePlaylist(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public void RemovePlaylist_WhenUserDoesNotOwnPlaylist_ShouldNotRemove()
    {
        // Arrange
        var otherUser = User.Create(
            new Email("other@example.com"),
            new PasswordHash("Password123!")
        );
        otherUser.Activate();
            
        var otherUserPlaylist = otherUser.CreatePlaylist().Value;
        _activeUser.Playlists.Should().BeEmpty();

        // Act
        var result = _activeUser.RemovePlaylist(otherUserPlaylist.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.UserDoesNotHavePlaylist);
        otherUser.Playlists.Should().ContainSingle();
    }

    [Fact]
    public void Playlists_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        _activeUser.CreatePlaylist();
        _activeUser.CreatePlaylist();

        // Act & Assert
        _activeUser.Playlists.Should().HaveCount(2);
        _activeUser.Playlists.Should().BeAssignableTo<IReadOnlyCollection<Playlist>>();
    }
}