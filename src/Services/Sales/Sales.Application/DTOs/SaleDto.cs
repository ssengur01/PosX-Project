namespace Sales.Application.DTOs;

public record SaleDto
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid? CustomerId { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = string.Empty;
    public List<SaleItemDto> Items { get; init; } = new();
    public List<PaymentDto> Payments { get; init; } = new();
}

public record SaleItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxRate { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal Total { get; init; }
}

public record PaymentDto
{
    public Guid Id { get; init; }
    public string Method { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? TransactionId { get; init; }
    public bool IsSuccessful { get; init; }
    public DateTime PaymentDate { get; init; }
}

public record CreateSaleDto(
    Guid EmployeeId,
    Guid? CustomerId,
    List<CreateSaleItemDto> Items);

public record CreateSaleItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TaxRate);

public record ShiftDto(
    Guid Id,
    Guid EmployeeId,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal StartingCash,
    decimal? EndingCash,
    decimal ExpectedCash,
    decimal Difference,
    bool IsOpen,
    string? Notes,
    string Currency,
    List<CashMovementDto> CashMovements);

public record CashMovementDto(
    Guid Id,
    string Type,
    decimal Amount,
    string Reason,
    DateTime Timestamp,
    string Currency);
