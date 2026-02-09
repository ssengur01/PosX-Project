using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Identity.Application.Commands.Register;
using Identity.Application.Commands.Login;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Persistence;

namespace Identity.IntegrationTests;

public class IdentityControllerTests : IClassFixture<IdentityApiFactory>
{
    private readonly HttpClient _client;
    private readonly IdentityApiFactory _factory;

    public IdentityControllerTests(IdentityApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedAndAuthResponse()
    {
        // Arrange
        await SeedDefaultRole();

        var command = new RegisterCommand(
            Email: "newuser@test.com",
            Username: "newuser",
            Password: "Password123!",
            FirstName: "New",
            LastName: "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be("newuser@test.com");
        authResponse.User.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        await SeedDefaultRole();
        await SeedTestUser("existing@test.com", "existinguser");

        var command = new RegisterCommand(
            Email: "existing@test.com",
            Username: "differentuser",
            Password: "Password123!",
            FirstName: "Test",
            LastName: "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithExistingUsername_ReturnsBadRequest()
    {
        // Arrange
        await SeedDefaultRole();
        await SeedTestUser("user@test.com", "existinguser");

        var command = new RegisterCommand(
            Email: "different@test.com",
            Username: "existinguser",
            Password: "Password123!",
            FirstName: "Test",
            LastName: "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndAuthResponse()
    {
        // Arrange
        await SeedDefaultRole();
        await SeedTestUser("loginuser@test.com", "loginuser", "Password123!");

        var command = new LoginCommand(
            EmailOrUsername: "loginuser",
            Password: "Password123!",
            IpAddress: null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Username.Should().Be("loginuser");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        await SeedDefaultRole();
        await SeedTestUser("user@test.com", "testuser", "CorrectPassword123!");

        var command = new LoginCommand(
            EmailOrUsername: "testuser",
            Password: "WrongPassword!",
            IpAddress: null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsBadRequest()
    {
        // Arrange
        var command = new LoginCommand(
            EmailOrUsername: "nonexistent",
            Password: "Password123!",
            IpAddress: null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsOkAndNewAuthResponse()
    {
        // Arrange
        await SeedDefaultRole();
        var (accessToken, refreshToken) = await RegisterAndGetTokens();

        var request = new { Token = refreshToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new { Token = "invalid-refresh-token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task SeedDefaultRole()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Clear existing data
        context.Roles.RemoveRange(context.Roles);
        await context.SaveChangesAsync();

        // Seed Cashier role (default role for registration)
        var cashierRole = new Role("Cashier", "Cashier role", RoleType.Cashier);
        context.Roles.Add(cashierRole);
        await context.SaveChangesAsync();
    }

    private async Task SeedTestUser(string email, string username, string password = "Password123!")
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Check if user already exists
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return;
        }

        // Hash password (simple hash for testing - in production use proper hashing)
        var passwordHasher = scope.ServiceProvider.GetRequiredService<Identity.Application.Interfaces.IPasswordHasher>();
        var hashedPassword = passwordHasher.HashPassword(password);

        var user = new User(email, username, hashedPassword, "Test", "User");
        context.Users.Add(user);

        // Assign Cashier role
        var cashierRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Cashier");
        if (cashierRole != null)
        {
            user.AddRole(cashierRole);
        }

        await context.SaveChangesAsync();
    }

    private async Task<(string accessToken, string refreshToken)> RegisterAndGetTokens()
    {
        var command = new RegisterCommand(
            Email: $"user{Guid.NewGuid():N}@test.com",
            Username: $"user{Guid.NewGuid():N}",
            Password: "Password123!",
            FirstName: "Test",
            LastName: "User");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return (authResponse!.Token, authResponse.RefreshToken);
    }
}
