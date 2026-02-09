namespace PosX.Desktop.ApiClient.Models;

public record InventoryItemDto(
    Guid Id,
    Guid ProductId,
    int Quantity,
    int ReorderLevel,
    bool IsLowStock,
    DateTime LastUpdated);

public record AdjustStockRequest(
    Guid ProductId,
    int NewQuantity,
    string Reason,
    string PerformedBy);
