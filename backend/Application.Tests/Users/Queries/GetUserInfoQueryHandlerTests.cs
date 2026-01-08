using FluentAssertions;

using Application.Users.Queries.GetUserInfo;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Queries;

public class GetUserInfoQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnUserInfo_WhenUserExists()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetUserInfoQuery(user.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("test@example.com");
        result.Value.FullName.Should().Be("Test User");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var query = new GetUserInfoQuery(Guid.NewGuid());

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }
}
