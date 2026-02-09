using BuildingBlocks.Common.Common;
using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.Register;

public record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName) : IRequest<Result<AuthResponse>>;
