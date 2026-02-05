using FluentAssertions;

using Application.Moderators.Queries.GetModeratorInfo;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Queries;

public class GetModeratorInfoQueryHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldReturnModeratorInfo_WhenModeratorExists()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Moderator Name",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var query = new GetModeratorInfoQuery(moderator.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("moderator@example.com");
        result.Value.FullName.Should().Be("Moderator Name");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var query = new GetModeratorInfoQuery(Guid.NewGuid());

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
}
