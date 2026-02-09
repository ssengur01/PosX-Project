using MediatR;

namespace Sales.Application.Commands.CloseShift;

public record CloseShiftCommand(
    Guid ShiftId,
    decimal EndingCash,
    string Currency,
    string? Notes
) : IRequest<Unit>;
