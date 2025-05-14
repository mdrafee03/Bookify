using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FluentAssertions;
using Xunit;

namespace Bookify.Domain.UnitTests.Users;

public class UserTest : BaseTest
{
    [Fact]
    public void CreateUser_Should_SetProperties()
    {
        // Act
        var user = User.CreateUser(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
    }

    [Fact]
    public void CreateUser_Should_RaiseUserCreatedDomainEvent()
    {
        // Act
        var user = User.CreateUser(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        var domainEvents = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);

        domainEvents.Should().NotBeNull();

        domainEvents.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void CreateUser_Should_AddRegisteredRoleToUser()
    {
        // Act
        var user = User.CreateUser(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        var roles = user.UserRoles;

        roles.Should().NotBeNull();
        roles.Should().Contain(Role.Registered);
    }
}
