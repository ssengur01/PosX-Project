namespace BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// Base class for integration events
/// </summary>
public abstract class IntegrationEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Date and time when the event was created
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
