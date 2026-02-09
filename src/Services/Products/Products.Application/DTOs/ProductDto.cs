namespace Products.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string SKU,
    string? Barcode,
    decimal Price,
    decimal Cost,
    string Currency,
    int StockQuantity,
    int MinimumStockLevel,
    bool IsActive,
    bool IsLowStock,
    Guid CategoryId,
    string CategoryName);

public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    Guid? ParentCategoryId);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
