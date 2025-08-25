using System.Diagnostics;
using AzureTelemetry.Infrastructure.Services;
using AzureTelemetry.Worker.Diagnostics;

namespace AzureTelemetry.Worker;

public class Worker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    static readonly ActivitySource ActivitySource = new(Defaults.Telemetry.ServiceName);
    
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            using var activity = ActivitySource.StartActivity("Example worker activity");
            
            using var scope = scopeFactory.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
            var message = await queueService.Receive(token);
            if (message != MessageForWorker.Empty)
            {
                ApplicationDiagnostics.MessageSeenCounter.Add(1);
                
                await Task.Delay(5000, token);
                var repo = scope.ServiceProvider.GetRequiredService<IDataRepository>();
                await repo.StoreAsync(message.Data, token);
            }

            await Task.Delay(1000, token);
        }
    }
}