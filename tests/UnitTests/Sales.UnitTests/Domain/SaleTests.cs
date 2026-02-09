using FluentAssertions;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.ValueObjects;

namespace Sales.UnitTests.Domain;

public class SaleTests
{
    [Fact]
    public void Constructor_ValidData_CreatesSale()
    {
        // Arrange
        var saleNumber = "SALE-001";
        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        // Act
        var sale = new Sale(saleNumber, employeeId, customerId);

        // Assert
        sale.Id.Should().NotBeEmpty();
        sale.SaleNumber.Should().Be(saleNumber);
        sale.EmployeeId.Should().Be(employeeId);
        sale.CustomerId.Should().Be(customerId);
        sale.Status.Should().Be(SaleStatus.Pending);
        sale.Subtotal.Should().Be(Money.Zero());
        sale.Total.Should().Be(Money.Zero());
        sale.Items.Should().BeEmpty();
        sale.Payments.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ValidItem_AddsItemAndRecalculatesTotals()
    {
        // Arrange
        var sale = CreateTestSale();
        var productId = Guid.NewGuid();
        var unitPrice = new Money(100m, "TRY");
        var taxRate = new TaxRate(18m, "KDV %18");
        var discount = Discount.None;

        // Act
        sale.AddItem(productId, "Test Product", 2, unitPrice, taxRate, discount);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.Subtotal.Should().Be(new Money(200m, "TRY"));
    }

    [Fact]
    public void AddItem_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));
        sale.Complete();

        // Act & Assert
        var act = () => sale.AddItem(Guid.NewGuid(), "Product 2", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add items to a non-pending sale");
    }

    [Fact]
    public void RemoveItem_ExistingItem_RemovesItemAndRecalculatesTotals()
    {
        // Arrange
        var sale = CreateTestSale();
        var productId = Guid.NewGuid();
        sale.AddItem(productId, "Product 1", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddItem(Guid.NewGuid(), "Product 2", 1, new Money(50m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);

        var itemToRemove = sale.Items.First();

        // Act
        sale.RemoveItem(itemToRemove.Id);

        // Assert
        sale.Items.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveItem_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        var itemId = sale.Items.First().Id;
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));
        sale.Complete();

        // Act & Assert
        var act = () => sale.RemoveItem(itemId);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot remove items from a non-pending sale");
    }

    [Fact]
    public void ApplyDiscount_ValidDiscount_AppliesDiscountAndRecalculatesTotals()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        var discount = new Discount(10m, DiscountType.Percentage); // 10% discount

        // Act
        sale.ApplyDiscount(discount);

        // Assert
        sale.DiscountAmount.Amount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void AddPayment_ValidPayment_AddsPayment()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        var paymentAmount = new Money(118m, "TRY");

        // Act
        sale.AddPayment(PaymentMethod.Cash, paymentAmount);

        // Assert
        sale.Payments.Should().HaveCount(1);
        sale.Payments.First().Amount.Should().Be(paymentAmount);
    }

    [Fact]
    public void Complete_WithSufficientPayment_CompletesSale()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));

        // Act
        sale.Complete();

        // Assert
        sale.Status.Should().Be(SaleStatus.Completed);
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_WithoutItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();

        // Act & Assert
        var act = () => sale.Complete();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot complete sale without items");
    }

    [Fact]
    public void Complete_WithInsufficientPayment_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(50m, "TRY")); // Not enough

        // Act & Assert
        var act = () => sale.Complete();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient payment");
    }

    [Fact]
    public void Complete_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));
        sale.Complete();

        // Act & Assert
        var act = () => sale.Complete();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending sales can be completed");
    }

    [Fact]
    public void Cancel_PendingSale_CancelsSale()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        var reason = "Customer changed mind";

        // Act
        sale.Cancel(reason);

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.Notes.Should().Be(reason);
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));
        sale.Complete();

        // Act & Assert
        var act = () => sale.Cancel("reason");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending sales can be cancelled");
    }

    [Fact]
    public void ProcessRefund_CompletedSale_RefundsSale()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));
        sale.Complete();
        var reason = "Defective product";

        // Act
        sale.ProcessRefund(reason);

        // Assert
        sale.Status.Should().Be(SaleStatus.Refunded);
        sale.Notes.Should().Contain("Refunded");
        sale.Notes.Should().Contain(reason);
    }

    [Fact]
    public void ProcessRefund_WhenNotCompleted_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = CreateTestSale();

        // Act & Assert
        var act = () => sale.ProcessRefund("reason");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only completed sales can be refunded");
    }

    [Fact]
    public void GetTotalPaid_WithPayments_ReturnsCorrectTotal()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(100m, "TRY"));
        sale.AddPayment(PaymentMethod.CreditCard, new Money(50m, "TRY"));

        // Act
        var totalPaid = sale.GetTotalPaid();

        // Assert
        totalPaid.Should().Be(new Money(150m, "TRY"));
    }

    [Fact]
    public void GetChange_WhenOverpaid_ReturnsChange()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        sale.AddPayment(PaymentMethod.Cash, new Money(200m, "TRY"));

        // Act
        var change = sale.GetChange();

        // Assert
        change.Amount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetChange_WhenExactPayment_ReturnsZero()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, new Money(100m, "TRY"), new TaxRate(18m, "KDV %18"), Discount.None);
        var total = sale.Total;
        sale.AddPayment(PaymentMethod.Cash, total);

        // Act
        var change = sale.GetChange();

        // Assert
        change.Should().Be(Money.Zero());
    }

    private Sale CreateTestSale()
    {
        return new Sale("SALE-TEST-001", Guid.NewGuid());
    }
}
