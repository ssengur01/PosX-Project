using Sales.Domain.Enums;
using Sales.Domain.ValueObjects;

namespace Sales.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public Money Amount { get; private set; }
    public string? TransactionId { get; private set; }
    public bool IsSuccessful { get; private set; }
    public DateTime PaymentDate { get; private set; }

    private Payment()
    {
        Amount = Money.Zero();
    }

    public Payment(Guid saleId, PaymentMethod method, Money amount, string? transactionId = null)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        Method = method;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        TransactionId = transactionId;
        IsSuccessful = true;
        PaymentDate = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        IsSuccessful = false;
    }
}
