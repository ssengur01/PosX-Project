using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Commands.AdjustStock;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.InventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        if (item == null)
        {
            item = new InventoryItem(request.ProductId, request.NewQuantity, 10);
            await _unitOfWork.InventoryRepository.AddAsync(item, cancellationToken);
        }
        else
        {
            item.AdjustStock(request.NewQuantity, request.Reason, request.PerformedBy);
            await _unitOfWork.InventoryRepository.UpdateAsync(item, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
