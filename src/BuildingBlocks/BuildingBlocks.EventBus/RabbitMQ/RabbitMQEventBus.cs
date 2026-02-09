using System.Text;
using System.Text.Json;
using BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.EventBus.RabbitMQ;

/// <summary>
/// RabbitMQ implementation of IEventBus
/// </summary>
public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQEventBus> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchangeName;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;

    public RabbitMQEventBus(
        IServiceProvider serviceProvider,
        ILogger<RabbitMQEventBus> logger,
        string hostName,
        string userName,
        string password,
        string exchangeName = "posx_event_bus")
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _exchangeName = exchangeName;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        var eventName = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: eventName,
            body: body,
            mandatory: true,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published event {EventName} with ID {EventId}", eventName, @event.Id);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(h => h == handlerType))
        {
            _logger.LogWarning("Handler {HandlerType} already registered for event {EventName}", handlerType.Name, eventName);
            return;
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsume(eventName);

        _logger.LogInformation("Subscribed to event {EventName} with handler {HandlerType}", eventName, handlerType.Name);
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (_handlers.ContainsKey(eventName))
        {
            _handlers[eventName].Remove(handlerType);

            if (_handlers[eventName].Count == 0)
            {
                _handlers.Remove(eventName);
                _eventTypes.Remove(typeof(T));
            }
        }

        _logger.LogInformation("Unsubscribed from event {EventName} with handler {HandlerType}", eventName, handlerType.Name);
    }

    private void StartBasicConsume(string eventName)
    {
        var queueName = $"posx_queue_{eventName}";

        _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false).GetAwaiter().GetResult();

        _channel.QueueBindAsync(
            queue: queueName,
            exchange: _exchangeName,
            routingKey: eventName).GetAwaiter().GetResult();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Consumer_Received;

        _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer).GetAwaiter().GetResult();
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message);
            await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {EventName}", eventName);
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (!_handlers.ContainsKey(eventName))
        {
            _logger.LogWarning("No handlers registered for event {EventName}", eventName);
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        foreach (var handlerType in _handlers[eventName])
        {
            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler == null)
            {
                _logger.LogWarning("Handler {HandlerType} not found in service provider", handlerType.Name);
                continue;
            }

            var eventType = _eventTypes.FirstOrDefault(t => t.Name == eventName);
            if (eventType == null)
            {
                _logger.LogWarning("Event type not found for {EventName}", eventName);
                continue;
            }

            var integrationEvent = JsonSerializer.Deserialize(message, eventType);
            var concreteHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
            var handleMethod = concreteHandlerType.GetMethod("Handle");

            if (handleMethod != null && integrationEvent != null)
            {
                await (Task)handleMethod.Invoke(handler, new[] { integrationEvent, CancellationToken.None })!;
            }
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
