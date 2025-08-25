using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AzureTelemetry.Infrastructure.Services;

public record MessageForWorker(string Data, string CorrelationId, string? TraceId, string? SpanId)
{
    public static MessageForWorker Empty => new(string.Empty, string.Empty, string.Empty, string.Empty);
}

public interface IQueueService
{
    public Task Send(MessageForWorker message, CancellationToken token);
    public Task<MessageForWorker> Receive(CancellationToken token);
}

public class QueueService(AzureService azureService) : IQueueService
{
    readonly QueueClient _queueClient = azureService.QueueServiceClient.GetQueueClient(Defaults.Queues.MessageTransport);

    public async Task Send(MessageForWorker message, CancellationToken token)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: token);
        await _queueClient.SendMessageAsync(BinaryData.FromString(JsonSerializer.Serialize(message)), cancellationToken: token);
    }
    
    public async Task<MessageForWorker> Receive(CancellationToken token)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: token);

        QueueMessage queueMessage = await _queueClient.ReceiveMessageAsync(cancellationToken: token);

        if (queueMessage?.MessageText is null)
        {
            return MessageForWorker.Empty;
        }
        
        var message = JsonSerializer.Deserialize<MessageForWorker>(queueMessage.MessageText);
        await _queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, token);

        return message!;
    }
}