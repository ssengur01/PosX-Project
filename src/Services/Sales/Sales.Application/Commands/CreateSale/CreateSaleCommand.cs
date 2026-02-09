using MediatR;
using Sales.Application.DTOs;

namespace Sales.Application.Commands.CreateSale;

public record CreateSaleCommand(
    Guid EmployeeId,
    Guid? CustomerId,
    List<CreateSaleItemDto> Items,
    List<PaymentItemDto> Payments) : IRequest<SaleDto>;

public record PaymentItemDto(
    string Method,
    decimal Amount,
    string? TransactionId);
