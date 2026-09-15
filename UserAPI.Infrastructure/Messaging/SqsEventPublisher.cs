using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Infrastructure.Messaging;

public sealed class SqsEventPublisher : IEventPublisher
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IAmazonSQS _sqs;

    public SqsEventPublisher(IAmazonSQS sqs)
    {
        _sqs = sqs;
    }

    public async Task PublishAsync<T>(
        T @event,
        string queueName,
        CancellationToken ct = default)
    {
        var queue = await _sqs.GetQueueUrlAsync(queueName, ct);
        var body = JsonSerializer.Serialize(@event, SerializerOptions);

        await _sqs.SendMessageAsync(
            new SendMessageRequest
            {
                QueueUrl = queue.QueueUrl,
                MessageBody = body
            },
            ct);
    }
}
