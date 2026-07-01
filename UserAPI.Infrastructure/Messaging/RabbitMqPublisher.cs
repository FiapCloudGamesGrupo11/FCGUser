using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
        _channel    = connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(T @event, string queueName, CancellationToken ct = default)
    {
        await _channel.QueueDeclareAsync(
            queue:      queueName,
            durable:    true,
            exclusive:  false,
            autoDelete: false,
            cancellationToken: ct);

        var body  = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        var props = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange:          string.Empty,
            routingKey:        queueName,
            mandatory:         false,
            basicProperties:   props,
            body:              body,
            cancellationToken: ct);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
