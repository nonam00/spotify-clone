using FluentAssertions;

using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Queries.GetUserInfo;
using Application.Users.Errors;

namespace Application.Tests.Users.Queries;

public class GetUserInfoQueryHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldReturnUserInfo_WhenUserExists()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
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
        result.Value.FullName.Should().Be("User");
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

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetUserInfoQuery(user.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var query = new GetUserInfoQuery(Guid.Empty);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }
}
