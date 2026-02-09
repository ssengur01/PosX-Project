using Sales.Domain.ValueObjects;

namespace Sales.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Subtotal { get; private set; }
    public TaxRate TaxRate { get; private set; }
    public Money TaxAmount { get; private set; }
    public Discount Discount { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money Total { get; private set; }

    private SaleItem()
    {
        ProductName = string.Empty;
        UnitPrice = Money.Zero();
        Subtotal = Money.Zero();
        TaxRate = TaxRate.Zero;
        TaxAmount = Money.Zero();
        Discount = Discount.None;
        DiscountAmount = Money.Zero();
        Total = Money.Zero();
    }

    public SaleItem(Guid saleId, Guid productId, string productName, int quantity, Money unitPrice, TaxRate taxRate, Discount discount)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Id = Guid.NewGuid();
        SaleId = saleId;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        TaxRate = taxRate ?? TaxRate.Zero;
        Discount = discount ?? Discount.None;

        CalculateTotals();
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Quantity = quantity;
        CalculateTotals();
    }

    public void ApplyDiscount(Discount discount)
    {
        Discount = discount ?? Discount.None;
        CalculateTotals();
    }

    private void CalculateTotals()
    {
        Subtotal = UnitPrice * Quantity;
        var discountedAmount = Discount.Apply(Subtotal);
        DiscountAmount = Subtotal - discountedAmount;
        var finalAmount = discountedAmount;
        TaxAmount = TaxRate.Calculate(finalAmount);
        Total = finalAmount + TaxAmount;
    }
}
