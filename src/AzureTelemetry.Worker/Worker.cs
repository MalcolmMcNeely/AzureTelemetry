using AzureTelemetry.Infrastructure.Services;

namespace AzureTelemetry.Worker;

public class Worker(IDataRepository repo, IQueueService queueService, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var message = await queueService.Receive(token);
            if (message != MessageForWorker.Empty)
            {
                await Task.Delay(500, token);
                await repo.StoreAsync(message.Data, token);
            }

            await Task.Delay(1000, token);
        }
    }
}