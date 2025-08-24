using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace AzureTelemetry.Infrastructure;

public class AzureService(string connectionString)
{
    public TableServiceClient TableServiceClient { get; } = new(connectionString);
    public BlobServiceClient BlobServiceClient { get; } = new(connectionString);
    public QueueServiceClient QueueServiceClient { get; } = new(connectionString);
}