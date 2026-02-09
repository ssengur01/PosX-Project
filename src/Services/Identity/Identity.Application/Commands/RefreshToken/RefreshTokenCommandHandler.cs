using BuildingBlocks.Common.Abstractions;
using BuildingBlocks.Common.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return Result.Failure<AuthResponse>("Invalid or expired refresh token");
        }

        var user = refreshToken.User;

        // Get roles
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Revoke old token
        refreshToken.Revoke(request.IpAddress ?? "0.0.0.0", newRefreshToken);
        _refreshTokenRepository.Update(refreshToken);

        // Save new refresh token
        var newRefreshTokenEntity = new Domain.Entities.RefreshToken(
            newRefreshToken,
            DateTime.UtcNow.AddDays(7),
            user.Id,
            request.IpAddress ?? "0.0.0.0");

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Email, user.Username, user.FirstName, user.LastName, roles));

        return Result.Success(response);
    }
}
