namespace Products.Domain.ValueObjects;

public record Barcode
{
    public string Value { get; init; }

    public Barcode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Barcode cannot be empty", nameof(value));

        if (value.Length < 8 || value.Length > 13)
            throw new ArgumentException("Barcode must be between 8 and 13 characters", nameof(value));

        Value = value.Trim();
    }
}
