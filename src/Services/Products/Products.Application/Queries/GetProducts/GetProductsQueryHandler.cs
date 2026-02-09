using BuildingBlocks.Common.Common;
using MediatR;
using Products.Application.DTOs;
using Products.Domain.Interfaces;

namespace Products.Application.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductDto>>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var products = await _productRepository.GetAllAsync(skip, request.PageSize, cancellationToken);
        var totalCount = await _productRepository.CountAsync(cancellationToken);

        var dtos = products.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.SKU, p.Barcode?.Value,
            p.Price.Amount, p.Cost.Amount, p.Price.Currency,
            p.StockQuantity, p.MinimumStockLevel, p.IsActive, p.IsLowStock(),
            p.CategoryId, p.Category.Name));

        var result = new PagedResult<ProductDto>(dtos, totalCount, request.Page, request.PageSize);
        return Result.Success(result);
    }
}
