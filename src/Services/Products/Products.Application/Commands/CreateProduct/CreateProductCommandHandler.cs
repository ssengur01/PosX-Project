using BuildingBlocks.Common.Abstractions;
using BuildingBlocks.Common.Common;
using MediatR;
using Products.Application.DTOs;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.Domain.ValueObjects;

namespace Products.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository,
        ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return Result.Failure<ProductDto>("Category not found");

        var existingProduct = await _productRepository.GetBySkuAsync(request.SKU, cancellationToken);
        if (existingProduct != null)
            return Result.Failure<ProductDto>("Product with this SKU already exists");

        var price = new Money(request.Price, request.Currency);
        var cost = new Money(request.Cost, request.Currency);
        Barcode? barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : new Barcode(request.Barcode);

        var product = new Product(request.Name, request.Description, request.SKU,
            price, cost, request.CategoryId, barcode, request.MinimumStockLevel);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new ProductDto(product.Id, product.Name, product.Description, product.SKU,
            product.Barcode?.Value, product.Price.Amount, product.Cost.Amount, product.Price.Currency,
            product.StockQuantity, product.MinimumStockLevel, product.IsActive, product.IsLowStock(),
            product.CategoryId, category.Name);

        return Result.Success(dto);
    }
}
