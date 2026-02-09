namespace BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// Event bus interface for publishing and subscribing to integration events
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an integration event
    /// </summary>
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;

    /// <summary>
    /// Subscribes to an integration event
    /// </summary>
    void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    /// <summary>
    /// Unsubscribes from an integration event
    /// </summary>
    void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}
