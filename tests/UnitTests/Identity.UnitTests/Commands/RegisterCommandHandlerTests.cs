using BuildingBlocks.Common.Abstractions;
using FluentAssertions;
using Identity.Application.Commands.Register;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using Moq;

namespace Identity.UnitTests.Commands;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new RegisterCommandHandler(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockPasswordHasher.Object,
            _mockTokenService.Object,
            _mockRefreshTokenRepository.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RegistersUserSuccessfully()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Username: "testuser",
            Password: "Password123!",
            FirstName: "John",
            LastName: "Doe");

        var cashierRole = new Role("Cashier", "Cashier role", RoleType.Cashier);

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(r => r.UsernameExistsAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockPasswordHasher
            .Setup(h => h.HashPassword(command.Password))
            .Returns("hashedPassword");

        _mockRoleRepository
            .Setup(r => r.GetByNameAsync("Cashier", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cashierRole);

        _mockTokenService
            .Setup(t => t.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
            .Returns("access_token");

        _mockTokenService
            .Setup(t => t.GenerateRefreshToken())
            .Returns("refresh_token");

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockRefreshTokenRepository
            .Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("access_token");
        result.Value.RefreshToken.Should().Be("refresh_token");
        result.Value.User.Email.Should().Be(command.Email);
        result.Value.User.Username.Should().Be(command.Username);

        _mockUserRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmailExists_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "existing@example.com",
            Username: "newuser",
            Password: "Password123!",
            FirstName: "John",
            LastName: "Doe");

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Email already exists");

        _mockUserRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UsernameExists_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "new@example.com",
            Username: "existinguser",
            Password: "Password123!",
            FirstName: "John",
            LastName: "Doe");

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(r => r.UsernameExistsAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Username already exists");

        _mockUserRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_HashesPassword()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Username: "testuser",
            Password: "Password123!",
            FirstName: "John",
            LastName: "Doe");

        var cashierRole = new Role("Cashier", "Cashier role", RoleType.Cashier);

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(r => r.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockPasswordHasher
            .Setup(h => h.HashPassword(command.Password))
            .Returns("hashedPassword");

        _mockRoleRepository
            .Setup(r => r.GetByNameAsync("Cashier", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cashierRole);

        _mockTokenService
            .Setup(t => t.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
            .Returns("access_token");

        _mockTokenService
            .Setup(t => t.GenerateRefreshToken())
            .Returns("refresh_token");

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockPasswordHasher.Verify(
            h => h.HashPassword(command.Password),
            Times.Once);
    }
}
