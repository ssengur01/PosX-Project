using MediatR;

namespace Sales.Application.Commands.ProcessRefund;

public record ProcessRefundCommand(
    Guid SaleId,
    decimal Amount,
    string Currency,
    string Reason,
    string ProcessedBy
) : IRequest<Guid>;
