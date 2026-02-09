using Sales.Domain.ValueObjects;

namespace Sales.Domain.Events;

public record SaleCompletedEvent(
    Guid SaleId,
    string SaleNumber,
    DateTime SaleDate,
    Guid EmployeeId,
    Guid? CustomerId,
    Money Total,
    List<SaleItemDto> Items);

public record SaleItemDto(
    Guid ProductId,
    int Quantity,
    Money UnitPrice,
    Money Total);
