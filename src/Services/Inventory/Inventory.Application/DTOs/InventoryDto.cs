namespace Inventory.Application.DTOs;

public record InventoryDto(
    Guid Id,
    Guid ProductId,
    int Quantity,
    int ReorderLevel,
    bool IsLowStock,
    DateTime LastUpdated);

public record StockMovementDto(
    Guid Id,
    string Type,
    int Quantity,
    string Reason,
    string PerformedBy,
    DateTime MovementDate);
