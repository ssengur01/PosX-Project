using BuildingBlocks.Common.Abstractions;
using BuildingBlocks.Common.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email or username
        var user = await _userRepository.GetByEmailAsync(request.EmailOrUsername, cancellationToken)
                   ?? await _userRepository.GetByUsernameAsync(request.EmailOrUsername, cancellationToken);

        if (user == null)
        {
            return Result.Failure<AuthResponse>("Invalid credentials");
        }

        // Check if account is locked
        if (user.IsLocked())
        {
            return Result.Failure<AuthResponse>("Account is locked. Please try again later.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResponse>("Invalid credentials");
        }

        // Record successful login
        user.RecordSuccessfulLogin();

        // Get roles
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new Domain.Entities.RefreshToken(
            refreshToken,
            DateTime.UtcNow.AddDays(7),
            user.Id,
            request.IpAddress ?? "0.0.0.0");

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Email, user.Username, user.FirstName, user.LastName, roles));

        return Result.Success(response);
    }
}
