using Sales.Domain.ValueObjects;

namespace Sales.Domain.Entities;

public class Refund
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public Money Amount { get; private set; }
    public string Reason { get; private set; }
    public string ProcessedBy { get; private set; }
    public DateTime RefundDate { get; private set; }

    private Refund()
    {
        Amount = Money.Zero();
        Reason = string.Empty;
        ProcessedBy = string.Empty;
    }

    public Refund(Guid saleId, Money amount, string reason, string processedBy)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        ProcessedBy = processedBy ?? throw new ArgumentNullException(nameof(processedBy));
        RefundDate = DateTime.UtcNow;
    }
}
