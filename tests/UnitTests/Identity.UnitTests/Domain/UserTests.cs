using FluentAssertions;
using Identity.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_ValidData_CreatesUser()
    {
        // Arrange
        var email = "test@example.com";
        var username = "testuser";
        var passwordHash = "hashedpassword";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = new User(email, username, passwordHash, firstName, lastName);

        // Assert
        user.Email.Should().Be(email);
        user.Username.Should().Be(username);
        user.PasswordHash.Should().Be(passwordHash);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Status.Should().Be(UserStatus.Active);
        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public void UpdatePassword_NewPasswordHash_UpdatesPassword()
    {
        // Arrange
        var user = CreateTestUser();
        var newPasswordHash = "newhashedpassword";

        // Act
        user.UpdatePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
    }

    [Fact]
    public void UpdateProfile_NewNames_UpdatesProfile()
    {
        // Arrange
        var user = CreateTestUser();
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        user.UpdateProfile(newFirstName, newLastName);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);
    }

    [Fact]
    public void RecordSuccessfulLogin_UpdatesLastLoginAndResetsFailedAttempts()
    {
        // Arrange
        var user = CreateTestUser();
        user.RecordFailedLogin();
        user.RecordFailedLogin();

        // Act
        user.RecordSuccessfulLogin();

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.FailedLoginAttempts.Should().Be(0);
        user.LockedUntil.Should().BeNull();
    }

    [Fact]
    public void RecordFailedLogin_IncrementsFailedAttempts()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.RecordFailedLogin();

        // Assert
        user.FailedLoginAttempts.Should().Be(1);
        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public void RecordFailedLogin_FiveAttempts_LocksAccount()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        // Assert
        user.FailedLoginAttempts.Should().Be(5);
        user.Status.Should().Be(UserStatus.Locked);
        user.LockedUntil.Should().NotBeNull();
        user.LockedUntil.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void IsLocked_AccountLocked_ReturnsTrue()
    {
        // Arrange
        var user = CreateTestUser();
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        // Act
        var isLocked = user.IsLocked();

        // Assert
        isLocked.Should().BeTrue();
    }

    [Fact]
    public void IsLocked_AccountActive_ReturnsFalse()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var isLocked = user.IsLocked();

        // Assert
        isLocked.Should().BeFalse();
    }

    [Fact]
    public void AddRole_NewRole_AddsRole()
    {
        // Arrange
        var user = CreateTestUser();
        var role = new Role("Admin", "Administrator role", RoleType.Admin);

        // Act
        user.AddRole(role);

        // Assert
        user.UserRoles.Should().HaveCount(1);
        user.UserRoles.First().RoleId.Should().Be(role.Id);
    }

    [Fact]
    public void AddRole_DuplicateRole_DoesNotAddAgain()
    {
        // Arrange
        var user = CreateTestUser();
        var role = new Role("Admin", "Administrator role", RoleType.Admin);

        // Act
        user.AddRole(role);
        user.AddRole(role);

        // Assert
        user.UserRoles.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveRole_ExistingRole_RemovesRole()
    {
        // Arrange
        var user = CreateTestUser();
        var role = new Role("Admin", "Administrator role", RoleType.Admin);
        user.AddRole(role);

        // Act
        user.RemoveRole(role.Id);

        // Assert
        user.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void RecordSuccessfulLogin_UnlocksLockedAccount()
    {
        // Arrange
        var user = CreateTestUser();
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }
        user.Status.Should().Be(UserStatus.Locked);

        // Act
        user.RecordSuccessfulLogin();

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.FailedLoginAttempts.Should().Be(0);
        user.LockedUntil.Should().BeNull();
    }

    private User CreateTestUser()
    {
        return new User(
            "test@example.com",
            "testuser",
            "hashedpassword",
            "John",
            "Doe"
        );
    }
}
