namespace Employees.Domain.ValueObjects;

public record Commission
{
    public decimal Rate { get; init; } // Commission rate as percentage (e.g., 5 for 5%)
    public decimal MinimumSales { get; init; } // Minimum sales amount to qualify for commission

    public Commission(decimal rate, decimal minimumSales = 0)
    {
        if (rate < 0 || rate > 100)
            throw new ArgumentException("Commission rate must be between 0 and 100", nameof(rate));

        if (minimumSales < 0)
            throw new ArgumentException("Minimum sales cannot be negative", nameof(minimumSales));

        Rate = rate;
        MinimumSales = minimumSales;
    }

    public decimal Calculate(decimal salesAmount)
    {
        if (salesAmount < MinimumSales)
            return 0;

        return salesAmount * (Rate / 100);
    }
}
