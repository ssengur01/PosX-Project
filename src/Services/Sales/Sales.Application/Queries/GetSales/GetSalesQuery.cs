using MediatR;
using Sales.Application.DTOs;

namespace Sales.Application.Queries.GetSales;

public record GetSalesQuery(DateTime? StartDate, DateTime? EndDate, Guid? EmployeeId) : IRequest<List<SaleDto>>;
