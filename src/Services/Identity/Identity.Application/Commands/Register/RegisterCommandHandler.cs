using BuildingBlocks.Common.Abstractions;
using BuildingBlocks.Common.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if email exists
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result.Failure<AuthResponse>("Email already exists");
        }

        // Check if username exists
        if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken))
        {
            return Result.Failure<AuthResponse>("Username already exists");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new User(
            request.Email,
            request.Username,
            passwordHash,
            request.FirstName,
            request.LastName);

        // Assign default role (Cashier)
        var cashierRole = await _roleRepository.GetByNameAsync("Cashier", cancellationToken);
        if (cashierRole != null)
        {
            user.AddRole(cashierRole);
        }

        await _userRepository.AddAsync(user, cancellationToken);

        // Generate tokens
        var roles = cashierRole != null ? new List<string> { cashierRole.Name } : new List<string>();
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new Domain.Entities.RefreshToken(
            refreshToken,
            DateTime.UtcNow.AddDays(7),
            user.Id,
            "0.0.0.0");

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
