using AzureTelemetry.Infrastructure.Services;

namespace AzureTelemetry.Worker;

public class Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
            var message = await queueService.Receive(token);
            if (message != MessageForWorker.Empty)
            {
                await Task.Delay(500, token);
                var repo = scope.ServiceProvider.GetRequiredService<IDataRepository>();
                await repo.StoreAsync(message.Data, token);
            }

            await Task.Delay(1000, token);
        }
    }
}