using BuildingBlocks.Common.Abstractions;

namespace BuildingBlocks.Common.BaseClasses;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
