using BuildingBlocks.Common.BaseClasses;
using Products.Domain.ValueObjects;

namespace Products.Domain.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    public Barcode? Barcode { get; private set; }
    public Money Price { get; private set; }
    public Money Cost { get; private set; }
    public int StockQuantity { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        SKU = string.Empty;
        Price = Money.Zero();
        Cost = Money.Zero();
    }

    public Product(string name, string description, string sku, Money price, Money cost,
        Guid categoryId, Barcode? barcode = null, int minimumStockLevel = 10)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("Product SKU cannot be empty", nameof(sku));

        Name = name;
        Description = description ?? string.Empty;
        SKU = sku;
        Barcode = barcode;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Cost = cost ?? throw new ArgumentNullException(nameof(cost));
        CategoryId = categoryId;
        StockQuantity = 0;
        MinimumStockLevel = minimumStockLevel;
        IsActive = true;
    }

    public void UpdateDetails(string name, string description, Barcode? barcode)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Barcode = barcode;
    }

    public void UpdatePricing(Money price, Money cost)
    {
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Cost = cost ?? throw new ArgumentNullException(nameof(cost));
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new InvalidOperationException("Stock quantity cannot be negative");

        StockQuantity = quantity;
    }

    public void AdjustStock(int adjustment)
    {
        var newStock = StockQuantity + adjustment;
        if (newStock < 0)
            throw new InvalidOperationException("Cannot reduce stock below zero");

        StockQuantity = newStock;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public bool IsLowStock() => StockQuantity <= MinimumStockLevel;
}
