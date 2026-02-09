namespace Inventory.Domain.Events;

public record StockLevelLowEvent(
    Guid ProductId,
    int CurrentQuantity,
    int ReorderLevel,
    DateTime DetectedAt);
