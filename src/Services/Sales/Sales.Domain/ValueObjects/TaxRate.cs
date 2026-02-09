namespace Sales.Domain.ValueObjects;

public record TaxRate
{
    public decimal Rate { get; init; }
    public string Name { get; init; }

    public TaxRate(decimal rate, string name)
    {
        if (rate < 0 || rate > 100)
            throw new ArgumentException("Tax rate must be between 0 and 100", nameof(rate));

        Rate = rate;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Money Calculate(Money amount)
    {
        return amount * (Rate / 100);
    }

    public static TaxRate Zero => new(0, "No Tax");
    public static TaxRate Standard => new(20, "KDV %20");
    public static TaxRate Reduced => new(10, "KDV %10");
}
