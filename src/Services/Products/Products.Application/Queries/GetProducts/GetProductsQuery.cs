using BuildingBlocks.Common.Common;
using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Queries.GetProducts;

public record GetProductsQuery(int Page = 1, int PageSize = 20) : IRequest<Result<PagedResult<ProductDto>>>;
