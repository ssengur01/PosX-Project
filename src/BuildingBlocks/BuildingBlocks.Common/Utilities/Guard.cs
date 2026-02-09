using System.Runtime.CompilerServices;

namespace BuildingBlocks.Common.Utilities;

/// <summary>
/// Guard clauses for parameter validation
/// </summary>
public static class Guard
{
    /// <summary>
    /// Ensures that the value is not null
    /// </summary>
    public static T AgainstNull<T>(
        T? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class
    {
        if (value == null)
            throw new ArgumentNullException(paramName);

        return value;
    }

    /// <summary>
    /// Ensures that the string is not null or empty
    /// </summary>
    public static string AgainstNullOrEmpty(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", paramName);

        return value;
    }

    /// <summary>
    /// Ensures that the string is not null or whitespace
    /// </summary>
    public static string AgainstNullOrWhiteSpace(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);

        return value;
    }

    /// <summary>
    /// Ensures that the value is within the specified range
    /// </summary>
    public static T AgainstOutOfRange<T>(
        T value,
        T min,
        T max,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");

        return value;
    }

    /// <summary>
    /// Ensures that the value is not negative
    /// </summary>
    public static T AgainstNegative<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(default(T)) < 0)
            throw new ArgumentException("Value cannot be negative.", paramName);

        return value;
    }

    /// <summary>
    /// Ensures that the collection is not null or empty
    /// </summary>
    public static IEnumerable<T> AgainstNullOrEmpty<T>(
        IEnumerable<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string? paramName = null)
    {
        if (collection == null || !collection.Any())
            throw new ArgumentException("Collection cannot be null or empty.", paramName);

        return collection;
    }
}
