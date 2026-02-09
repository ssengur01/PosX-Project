using MediatR;

namespace Sales.Application.Commands.AddCashMovement;

public record AddCashMovementCommand(
    Guid ShiftId,
    string Type,
    decimal Amount,
    string Currency,
    string Reason
) : IRequest<Unit>;
