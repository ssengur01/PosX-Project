namespace BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// Interface for handling integration events
/// </summary>
/// <typeparam name="TIntegrationEvent">Type of integration event to handle</typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Handles the integration event
    /// </summary>
    Task Handle(TIntegrationEvent @event, CancellationToken cancellationToken = default);
}
