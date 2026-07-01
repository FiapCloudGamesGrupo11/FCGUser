namespace UserAPI.Domain.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, string queueName, CancellationToken ct = default);
}
