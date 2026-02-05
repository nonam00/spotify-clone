using FluentAssertions;

using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Tests.Users;

public class UserSongTests
{
    private readonly User _activeUser;
    private readonly User _inactiveUser;

    public UserSongTests()
    {
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("TestPassword123!");
            
        _activeUser = User.Create(email, passwordHash);
        _activeUser.Activate();
            
        _inactiveUser = User.Create(email, passwordHash);
    }

        
    [Fact]
    public void UploadedSongs_ShouldReturnEmptyCollectionInitially()
    {
        // Act
        var user = User.Create(new Email("email@example.com"), new PasswordHash("hashed_password"));

        // Assert
        user.UploadedSongs.Should().BeEmpty();
    }
    
    [Fact]
    public void UploadSong_WhenUserIsActive_ShouldUploadSuccessfully()
    {
        // Arrange
        const string title = "My New Song";
        const string author = "Me";
        var audioPath = new FilePath("my-song.mp3");
        var imagePath = new FilePath("my-song.jpg");

        // Act
        var result = _activeUser.UploadSong(title, author, audioPath, imagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.UploadedSongs.Should().ContainSingle();
        
        var uploadedSong = result.Value;
            
        uploadedSong.Title.Should().Be(title);
        uploadedSong.Author.Should().Be(author);
        uploadedSong.SongPath.Should().Be(audioPath);
        uploadedSong.ImagePath.Should().Be(imagePath);
        uploadedSong.UploaderId.Should().Be(_activeUser.Id);
        uploadedSong.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void UploadSong_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        const string title = "My New Song";
        const string author = "Me";
        var audioPath = new FilePath("my-song.mp3");
        var imagePath = new  FilePath("my-song.jpg");

        // Act
        var result = _inactiveUser.UploadSong(title, author, audioPath, imagePath);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
        _inactiveUser.UploadedSongs.Should().BeEmpty();
    }

    [Fact]
    public void UploadSong_WithInvalidParameters_ShouldReturnFailure()
    {
        // Arrange
        var audioPath = new FilePath("my-song.mp3");
        var imagePath = new  FilePath("my-song.jpg");

        // Act
        var result = _activeUser.UploadSong("", "Author", audioPath, imagePath);

        // Assert
        result.IsFailure.Should().BeTrue();
        _activeUser.UploadedSongs.Should().BeEmpty();
    }

    [Fact]
    public void UploadSong_ShouldAddSongToUploadedSongsCollection()
    {
        // Arrange
        var audioPath = new FilePath("my-song.mp3");
        var imagePath = new  FilePath("my-song.jpg");

        // Act
        var result = _activeUser.UploadSong("Song 1", "Artist", audioPath, imagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _activeUser.UploadedSongs.Should().Contain(result.Value);
    }
}