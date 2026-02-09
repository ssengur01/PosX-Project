namespace PosX.Desktop.ApiClient.Models;

public record CreateSaleRequest(
    Guid EmployeeId,
    Guid? CustomerId,
    List<CreateSaleItemRequest> Items,
    List<PaymentItemRequest> Payments);

public record CreateSaleItemRequest(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TaxRate);

public record PaymentItemRequest(
    string Method,
    decimal Amount,
    string? TransactionId);

public record SaleResponse(
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
    List<SaleItemResponse> Items,
    List<PaymentResponse> Payments);

public record SaleItemResponse(
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

public record PaymentResponse(
    Guid Id,
    string Method,
    decimal Amount,
    string? TransactionId,
    bool IsSuccessful,
    DateTime PaymentDate);

public record ProcessRefundRequest(
    decimal Amount,
    string Currency,
    string Reason,
    string ProcessedBy);

public record RefundResponse(
    Guid RefundId);

public record OpenShiftRequest(
    Guid EmployeeId,
    decimal StartingCash,
    string Currency);

public record OpenShiftResponse(
    Guid ShiftId);

public record CloseShiftRequest(
    decimal EndingCash,
    string Currency,
    string? Notes);

public record AddCashMovementRequest(
    string Type,
    decimal Amount,
    string Currency,
    string Reason);

public record ShiftResponse(
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
    List<CashMovementResponse> CashMovements);

public record CashMovementResponse(
    Guid Id,
    string Type,
    decimal Amount,
    string Reason,
    DateTime Timestamp,
    string Currency);
