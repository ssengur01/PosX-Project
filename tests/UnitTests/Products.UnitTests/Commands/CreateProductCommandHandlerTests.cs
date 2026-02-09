using BuildingBlocks.Common.Abstractions;
using FluentAssertions;
using Moq;
using Products.Application.Commands.CreateProduct;
using Products.Domain.Entities;
using Products.Domain.Interfaces;

namespace Products.UnitTests.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateProductCommandHandler(_mockRepository.Object, _mockCategoryRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProductSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", "Test Description");

        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: "Test Description",
            SKU: "TEST-001",
            Barcode: "1234567890123",
            Price: 99.99m,
            Cost: 50.00m,
            Currency: "TRY",
            CategoryId: categoryId,
            MinimumStockLevel: 10
        );

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mockRepository.Setup(r => r.GetBySkuAsync(command.SKU, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Test Product");
        result.Value.SKU.Should().Be("TEST-001");
        result.Value.Price.Should().Be(99.99m);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NegativePrice_ThrowsArgumentException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", "Test Description");

        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: "Test Description",
            SKU: "TEST-001",
            Barcode: null,
            Price: -10.00m,
            Cost: 5.00m,
            Currency: "TRY",
            CategoryId: categoryId,
            MinimumStockLevel: 10
        );

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mockRepository.Setup(r => r.GetBySkuAsync(command.SKU, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", "Test Description");

        var command = new CreateProductCommand(
            Name: "",
            Description: "Test Description",
            SKU: "TEST-001",
            Barcode: null,
            Price: 10.00m,
            Cost: 5.00m,
            Currency: "TRY",
            CategoryId: categoryId,
            MinimumStockLevel: 10
        );

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mockRepository.Setup(r => r.GetBySkuAsync(command.SKU, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
