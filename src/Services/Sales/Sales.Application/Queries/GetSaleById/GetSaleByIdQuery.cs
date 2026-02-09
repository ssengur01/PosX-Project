using MediatR;
using Sales.Application.DTOs;

namespace Sales.Application.Queries.GetSaleById;

public record GetSaleByIdQuery(Guid SaleId) : IRequest<SaleDto?>;
