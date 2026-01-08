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
        song!.IsPublished.Should().BeFalse();
    }
}
