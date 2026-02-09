using MediatR;

namespace Sales.Application.Commands.OpenShift;

public record OpenShiftCommand(
    Guid EmployeeId,
    decimal StartingCash,
    string Currency
) : IRequest<Guid>;
