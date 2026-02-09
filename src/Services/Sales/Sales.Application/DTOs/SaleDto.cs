namespace Sales.Application.DTOs;

public record SaleDto(
    Guid Id,
    string SaleNumber,
    DateTime SaleDate,
    Guid EmployeeId,
    Guid? CustomerId,
    string Status,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal Total,
    string Currency,
    List<SaleItemDto> Items,
    List<PaymentDto> Payments);

public record SaleItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal,
    decimal TaxRate,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal Total);

public record PaymentDto(
    Guid Id,
    string Method,
    decimal Amount,
    string? TransactionId,
    bool IsSuccessful,
    DateTime PaymentDate);

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
