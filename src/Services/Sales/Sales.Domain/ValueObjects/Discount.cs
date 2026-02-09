namespace Sales.Domain.ValueObjects;

public record Discount
{
    public decimal Amount { get; init; }
    public DiscountType Type { get; init; }

    private Discount() { }

    public Discount(decimal amount, DiscountType type)
    {
        if (amount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(amount));

        if (type == DiscountType.Percentage && amount > 100)
            throw new ArgumentException("Percentage discount cannot exceed 100%", nameof(amount));

        Amount = amount;
        Type = type;
    }

    public Money Apply(Money original)
    {
        return Type switch
        {
            DiscountType.Percentage => original * (1 - Amount / 100),
            DiscountType.Fixed => original - new Money(Amount, original.Currency),
            _ => original
        };
    }

    public static Discount None => new(0, DiscountType.Percentage);
}

public enum DiscountType
{
    Percentage = 0,
    Fixed = 1
}
