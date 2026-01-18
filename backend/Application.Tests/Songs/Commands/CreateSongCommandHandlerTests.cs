using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Songs.Commands.CreateSong;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Songs.Commands;

public class CreateSongCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldCreateSong_WhenValidData()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        var song = await Context.Songs.FirstOrDefaultAsync(s => s.Id == result.Value);
        
        song.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldCreateUnpublishedSong_ByDefault()
    {
        // Arrange
        var command = new CreateSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var song = await Context.Songs.FirstOrDefaultAsync(s => s.Id == result.Value);
        
        song.Should().NotBeNull();
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new CreateSongCommand(
            Guid.Empty,
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenTitleIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new CreateSongCommand(
            user.Id,
            "",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Title");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var longTitle = new string('a', 201);
        var command = new CreateSongCommand(
            user.Id,
            longTitle,
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Title");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenAuthorIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            "",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Author");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenAuthorExceedsMaxLength()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var longAuthor = new string('a', 201);
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            longAuthor,
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Author");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongPathIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongPath");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongPathExceedsMaxLength()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var longPath = new string('a', 501);
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            longPath,
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongPath");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenImagePathIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "song.mp3",
            "");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ImagePath");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenImagePathExceedsMaxLength()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var longPath = new string('a', 501);
        var command = new CreateSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "song.mp3",
            longPath);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ImagePath");
    }
}
