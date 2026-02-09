using Sales.Domain.Enums;
using Sales.Domain.ValueObjects;

namespace Sales.Domain.Entities;

public class Sale
{
    public Guid Id { get; private set; }
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public SaleStatus Status { get; private set; }
    public Money Subtotal { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money Total { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Sale()
    {
        SaleNumber = string.Empty;
        Subtotal = Money.Zero();
        DiscountAmount = Money.Zero();
        TaxAmount = Money.Zero();
        Total = Money.Zero();
    }

    public Sale(string saleNumber, Guid employeeId, Guid? customerId = null)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
        SaleDate = DateTime.UtcNow;
        EmployeeId = employeeId;
        CustomerId = customerId;
        Status = SaleStatus.Pending;
        Subtotal = Money.Zero();
        DiscountAmount = Money.Zero();
        TaxAmount = Money.Zero();
        Total = Money.Zero();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, string productName, int quantity, Money unitPrice, TaxRate taxRate, Discount discount)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending sale");

        var item = new SaleItem(Id, productId, productName, quantity, unitPrice, taxRate, discount);
        _items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from a non-pending sale");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotals();
        }
    }

    public void ApplyDiscount(Discount discount)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot apply discount to a non-pending sale");

        var discountedTotal = discount.Apply(Subtotal);
        DiscountAmount = Subtotal - discountedTotal;
        RecalculateTotals();
    }

    public void AddPayment(PaymentMethod method, Money amount, string? transactionId = null)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot add payment to a non-pending sale");

        var payment = new Payment(Id, method, amount, transactionId);
        _payments.Add(payment);
    }

    public void Complete()
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Only pending sales can be completed");

        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot complete sale without items");

        var totalPaid = _payments.Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);
        if (totalPaid < Total)
            throw new InvalidOperationException("Insufficient payment");

        Status = SaleStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Only pending sales can be cancelled");

        Status = SaleStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ProcessRefund(string reason)
    {
        if (Status != SaleStatus.Completed)
            throw new InvalidOperationException("Only completed sales can be refunded");

        Status = SaleStatus.Refunded;
        Notes = $"Refunded: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    public Money GetTotalPaid()
    {
        return _payments.Where(p => p.IsSuccessful).Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);
    }

    public Money GetChange()
    {
        var totalPaid = GetTotalPaid();
        return totalPaid > Total ? totalPaid - Total : Money.Zero();
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Aggregate(Money.Zero(), (sum, item) => sum + item.Subtotal);
        TaxAmount = _items.Aggregate(Money.Zero(), (sum, item) => sum + item.TaxAmount);
        Total = _items.Aggregate(Money.Zero(), (sum, item) => sum + item.Total);
        Total = Total - DiscountAmount;
        UpdatedAt = DateTime.UtcNow;
    }
}
