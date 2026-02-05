using FluentAssertions;

using Domain.Errors;
using Domain.Models;
using Domain.Tests.Helpers;
using Domain.ValueObjects;

namespace Domain.Tests.Songs;

public class SongTests
{
    private readonly Guid _uploaderId = Guid.NewGuid();
    private readonly FilePath _testSongPath = new("test.mp3");
    private readonly FilePath _testImagePath = new("test.jpg");

    [Fact]
    public void Create_ShouldCreateSongWithCorrectProperties()
    {
        // Arrange
        const string title = "Test Song";
        const string author = "Test Artist";

        // Act
        var result = Song.Create(title, author, _testSongPath, _testImagePath, _uploaderId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var song = result.Value;
            
        song.Id.Should().NotBe(Guid.Empty);
        song.Title.Should().Be(title);
        song.Author.Should().Be(author);
        song.SongPath.Should().Be(_testSongPath);
        song.ImagePath.Should().Be(_testImagePath);
        song.UploaderId.Should().Be(_uploaderId);
        song.IsPublished.Should().BeFalse();
        song.MarkedForDeletion.Should().BeFalse();
        song.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ShouldReturnFailure(string? title)
    {
        // Act
        var result = Song.Create(title, "Artist", _testSongPath, _testImagePath);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.TitleCannotBeEmpty);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyAuthor_ShouldReturnFailure(string? author)
    {
        // Act
        var result = Song.Create("Title", author, _testSongPath, _testImagePath);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.AuthorCannotBeEmpty);
    }

    [Fact]
    public void Create_ShouldTrimTitleAndAuthor()
    {
        // Arrange
        const string title = "  Test Song  ";
        const string author = "  Test Artist  ";

        // Act
        var result = Song.Create(title, author, _testSongPath, _testImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var song = result.Value;
        song.Title.Should().Be("Test Song");
        song.Author.Should().Be("Test Artist");
    }

    [Fact]
    public void Create_WithNullUploaderId_ShouldBeAllowed()
    {
        // Act
        var result = Song.Create("Title", "Artist", _testSongPath, _testImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UploaderId.Should().BeNull();
    }

    [Fact]
    public void Publish_WhenSongIsNotPublished_ShouldPublishSuccessfully()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        
        // Act
        var result = song.Publish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        song.IsPublished.Should().BeTrue();
    }

    [Fact]
    public void Publish_WhenSongIsAlreadyPublished_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: true);
        
        // Act
        var result = song.Publish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.AlreadyPublished);
        song.IsPublished.Should().BeTrue();
    }

    [Fact]
    public void Publish_WhenSongIsMarkedForDeletion_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        song.MarkForDeletion();

        // Act
        var result = song.Publish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.CannotPublishMarkedForDeletion);
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void Unpublish_WhenSongIsPublished_ShouldUnpublishSuccessfully()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: true);

        // Act
        var result = song.Unpublish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void Unpublish_WhenSongIsAlreadyUnpublished_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        
        // Act
        var result = song.Unpublish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.AlreadyUnpublished);
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void MarkForDeletion_WhenSongIsUnpublished_ShouldMarkSuccessfully()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        
        // Act
        var result = song.MarkForDeletion();

        // Assert
        result.IsSuccess.Should().BeTrue();
        song.MarkedForDeletion.Should().BeTrue();
    }

    [Fact]
    public void MarkForDeletion_WhenSongIsPublished_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: true);

        // Act
        var result = song.MarkForDeletion();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.CannotDeletePublished);
        song.MarkedForDeletion.Should().BeFalse();
    }

    [Fact]
    public void MarkForDeletion_WhenAlreadyMarkedForDeletion_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        song.MarkForDeletion();

        // Act
        var result = song.MarkForDeletion();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.AlreadyMarkedForDeletion);
        song.MarkedForDeletion.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldInitializeAsUnpublishedAndNotMarkedForDeletion()
    {
        // Act
        var song = SongHelpers.CreateSong();

        // Assert
        song.IsPublished.Should().BeFalse();
        song.MarkedForDeletion.Should().BeFalse();
    }
}