using BuildingBlocks.Common.Common;
using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string SKU,
    string? Barcode,
    decimal Price,
    decimal Cost,
    string Currency,
    Guid CategoryId,
    int MinimumStockLevel = 10) : IRequest<Result<ProductDto>>;
