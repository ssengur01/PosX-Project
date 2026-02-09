using MediatR;
using Sales.Domain.Interfaces;
using Sales.Domain.Entities;
using Sales.Domain.ValueObjects;

namespace Sales.Application.Commands.ProcessRefund;

public class ProcessRefundCommandHandler : IRequestHandler<ProcessRefundCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IRefundRepository _refundRepository;

    public ProcessRefundCommandHandler(ISaleRepository saleRepository, IRefundRepository refundRepository)
    {
        _saleRepository = saleRepository;
        _refundRepository = refundRepository;
    }

    public async Task<Guid> Handle(ProcessRefundCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.SaleId} not found");

        var amount = new Money(request.Amount, request.Currency);

        if (amount > sale.Total)
            throw new InvalidOperationException("Refund amount cannot exceed sale total");

        sale.ProcessRefund(request.Reason);

        var refund = new Refund(
            request.SaleId,
            amount,
            request.Reason,
            request.ProcessedBy
        );

        await _refundRepository.AddAsync(refund);
        await _saleRepository.UpdateAsync(sale);

        return refund.Id;
    }
}
