namespace Employees.Domain.ValueObjects;

public record Salary
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Salary(decimal amount, string currency = "TRY")
    {
        if (amount < 0)
            throw new ArgumentException("Salary amount cannot be negative", nameof(amount));

        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
