using Domain.Errors;
using FluentAssertions;

using Domain.Models;
using Domain.Tests.Helpers;
using Domain.ValueObjects;

namespace Domain.Tests.Users;

public class UserLikeSongTests
{
    private readonly User _activeUser;
    private readonly User _inactiveUser;
    private readonly Song _publishedSong;
    private readonly Song _unpublishedSong;
    
    public UserLikeSongTests()
    {
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("TestPassword123!");
            
        _activeUser = User.Create(email, passwordHash);
        _activeUser.Activate();
            
        _inactiveUser = User.Create(email, passwordHash);

        _publishedSong = SongHelpers.CreateSong(isPublished: true);
        _unpublishedSong = SongHelpers.CreateSong();
    }

    [Fact]
    public void UserLikedSongs_ShouldReturnEmptyCollectionInitially()
    {
        // Act
        var user = User.Create(new Email("email@example.com"), new PasswordHash("hashed_password"));
        
        // Assert
        user.UserLikedSongs.Should().BeEmpty();
    }

    
    [Fact]
    public void LikeSong_WhenUserIsActiveAndSongIsPublished_ShouldLikeSuccessfully()
    {
        // Act
        var result = _activeUser.LikeSong(_publishedSong);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.UserLikedSongs.Should().ContainSingle();
        var likedSong = _activeUser.UserLikedSongs.First();
        likedSong.UserId.Should().Be(_activeUser.Id);
        likedSong.SongId.Should().Be(_publishedSong.Id);
    }

    [Fact]
    public void LikeSong_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Act
        var result = _inactiveUser.LikeSong(_publishedSong);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
        _inactiveUser.UserLikedSongs.Should().BeEmpty();
    }

    [Fact]
    public void LikeSong_WhenSongIsUnpublished_ShouldReturnFailure()
    {
        // Act
        var result = _activeUser.LikeSong(_unpublishedSong);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.CannotLikedUnpublishedSong);
        _activeUser.UserLikedSongs.Should().BeEmpty();
    }

    [Fact]
    public void LikeSong_WhenSongIsAlreadyLiked_ShouldReturnFailure()
    {
        // Arrange
        _activeUser.LikeSong(_publishedSong);

        // Act
        var result = _activeUser.LikeSong(_publishedSong);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.SongAlreadyLiked);
        _activeUser.UserLikedSongs.Should().ContainSingle();
    }

    [Fact]
    public void LikeSong_ShouldAllowUserToLikeMultipleSongs()
    {
        // Arrange
        var song1 = SongHelpers.CreateSong(isPublished: true);
        var song2 = SongHelpers.CreateSong(isPublished: true);

        // Act
        var result1 = _activeUser.LikeSong(song1);
        var result2 = _activeUser.LikeSong(song2);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        _activeUser.UserLikedSongs.Should().HaveCount(2);
    }

    [Fact]
    public void UnlikeSong_WhenUserIsActiveAndSongIsLiked_ShouldUnlikeSuccessfully()
    {
        // Arrange
        _activeUser.LikeSong(_publishedSong);
        _activeUser.UserLikedSongs.Should().ContainSingle();

        // Act
        var result = _activeUser.UnlikeSong(_publishedSong.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.UserLikedSongs.Should().BeEmpty();
    }

    [Fact]
    public void UnlikeSong_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Act
        var result = _inactiveUser.UnlikeSong(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public void UnlikeSong_WhenSongIsNotLiked_ShouldReturnFailure()
    {
        // Act
        var result = _activeUser.UnlikeSong(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.SongNotLiked);
        _activeUser.UserLikedSongs.Should().BeEmpty();
    }
    
    
    [Fact]
    public void HasLikedSong_ShouldReturnCorrectState()
    {
        // Arrange
        _activeUser.LikeSong(_publishedSong);

        // Act & Assert
        // This is a private method, but we can test through Like/Unlike behavior
        _activeUser.LikeSong(_publishedSong).IsFailure.Should().BeTrue(); // Already liked
        _activeUser.UnlikeSong(_publishedSong.Id).IsSuccess.Should().BeTrue();
        _activeUser.LikeSong(_publishedSong).IsSuccess.Should().BeTrue(); // Can like again
    }
}