using MediatR;

namespace Inventory.Application.Commands.AdjustStock;

public record AdjustStockCommand(
    Guid ProductId,
    int NewQuantity,
    string Reason,
    string PerformedBy) : IRequest<bool>;
