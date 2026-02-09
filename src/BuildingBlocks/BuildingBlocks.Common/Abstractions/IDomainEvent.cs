namespace BuildingBlocks.Common.Abstractions;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Date and time when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
}
