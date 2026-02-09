using Sales.Domain.ValueObjects;

namespace Sales.Domain.Events;

public record RefundProcessedEvent(
    Guid RefundId,
    Guid SaleId,
    Money Amount,
    string Reason,
    string ProcessedBy,
    DateTime RefundDate);
