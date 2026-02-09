namespace PosX.Desktop.ApiClient.Models;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string SKU,
    decimal Price,
    decimal Cost,
    string Currency,
    int StockQuantity,
    int ReorderLevel,
    string? Barcode,
    Guid CategoryId,
    string CategoryName,
    bool IsActive);

public record PagedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
