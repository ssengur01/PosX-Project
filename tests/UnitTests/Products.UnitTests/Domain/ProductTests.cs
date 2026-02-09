using FluentAssertions;
using Products.Domain.Entities;
using Products.Domain.ValueObjects;

namespace Products.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Constructor_ValidData_CreatesProduct()
    {
        // Arrange
        var name = "Test Product";
        var sku = "TEST-001";
        var price = new Money(100m, "TRY");
        var cost = new Money(50m, "TRY");
        var categoryId = Guid.NewGuid();

        // Act
        var product = new Product(name, "Description", sku, price, cost, categoryId);

        // Assert
        product.Name.Should().Be(name);
        product.SKU.Should().Be(sku);
        product.Price.Should().Be(price);
        product.Cost.Should().Be(cost);
        product.CategoryId.Should().Be(categoryId);
        product.IsActive.Should().BeTrue();
        product.StockQuantity.Should().Be(0);
    }

    [Fact]
    public void UpdateStock_PositiveQuantity_IncreasesStock()
    {
        // Arrange
        var product = CreateTestProduct();
        var initialStock = product.StockQuantity;

        // Act
        product.UpdateStock(50);

        // Assert
        product.StockQuantity.Should().Be(initialStock + 50);
    }

    [Fact]
    public void UpdateStock_NegativeQuantityBeyondStock_ThrowsException()
    {
        // Arrange
        var product = CreateTestProduct();
        product.UpdateStock(10); // Set stock to 10

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.UpdateStock(-20));
    }

    [Fact]
    public void Deactivate_ActiveProduct_SetsIsActiveFalse()
    {
        // Arrange
        var product = CreateTestProduct();
        product.IsActive.Should().BeTrue();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdatePricing_ValidPrice_UpdatesPrice()
    {
        // Arrange
        var product = CreateTestProduct();
        var newPrice = new Money(150m, "TRY");
        var newCost = new Money(75m, "TRY");

        // Act
        product.UpdatePricing(newPrice, newCost);

        // Assert
        product.Price.Should().Be(newPrice);
        product.Cost.Should().Be(newCost);
    }

    private Product CreateTestProduct()
    {
        return new Product(
            "Test Product",
            "Test Description",
            "TEST-001",
            new Money(100m, "TRY"),
            new Money(50m, "TRY"),
            Guid.NewGuid()
        );
    }
}
