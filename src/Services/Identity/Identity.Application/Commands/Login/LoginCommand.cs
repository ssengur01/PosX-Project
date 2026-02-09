using BuildingBlocks.Common.Common;
using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.Login;

public record LoginCommand(
    string EmailOrUsername,
    string Password,
    string? IpAddress) : IRequest<Result<AuthResponse>>;
