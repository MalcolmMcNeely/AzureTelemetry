using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureTelemetry.Infrastructure.Services;

public class LookupEntity : ITableEntity
{
    public const string Partition = "lookup";
    
    public string PartitionKey { get; set; } = Partition;
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public required string Location { get; set; }
}

public interface IDataRepository
{
    public Task<string> StoreAsync(string data, CancellationToken token);
    public Task<string> RetrieveAsync(string id, CancellationToken token);
}

public class DataRepository(AzureService azureService) : IDataRepository
{
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Tables.DataLookup);
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.Containers.Data);
    
    public async Task<string> StoreAsync(string data, CancellationToken token)
    {
        var dataId = Guid.NewGuid().ToString();
        var dataLocation = Guid.NewGuid().ToString();

        await _tableClient.CreateIfNotExistsAsync(token);
        var lookupEntity = new LookupEntity { RowKey = dataId, Location = dataLocation};
        await _tableClient.AddEntityAsync(lookupEntity, token);

        await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: token);
        await _blobContainerClient.UploadBlobAsync(dataLocation, BinaryData.FromString(data), token);

        return dataId;
    }

    public async Task<string> RetrieveAsync(string id, CancellationToken token)
    {
        LookupEntity entity = await _tableClient.GetEntityAsync<LookupEntity>(LookupEntity.Partition, id, null, token);
        var blobName = entity.Location;
        
        var blobClient = _blobContainerClient.GetBlobClient(blobName);
        BlobDownloadInfo result = await blobClient.DownloadAsync(cancellationToken: token);

        var streamReader = new StreamReader(result.Content);
        return await streamReader.ReadToEndAsync(token);
    }
}
