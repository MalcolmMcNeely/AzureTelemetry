using System.Diagnostics;
using AzureTelemetry.Infrastructure.Services;
using AzureTelemetry.Worker.Diagnostics;
using OpenTelemetry;

namespace AzureTelemetry.Worker;

public class Worker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    static readonly ActivitySource ActivitySource = new(Defaults.Telemetry.ServiceName);

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
            var message = await queueService.Receive(token);
            if (message != MessageForWorker.Empty)
            {
                // convention should be {event type} {operation}
                using var activity = ActivitySource.StartActivity("messageForWorker consume", ActivityKind.Consumer,
                    parentContext: new ActivityContext(
                        ActivityTraceId.CreateFromString(message.TraceId),
                        ActivitySpanId.CreateFromString(message.SpanId),
                        ActivityTraceFlags.Recorded));

                Baggage.SetBaggage("correlationId", message.CorrelationId);

                ApplicationDiagnostics.MessageSeenCounter.Add(1);

                await Task.Delay(5000, token);
                var repo = scope.ServiceProvider.GetRequiredService<IDataRepository>();
                await repo.StoreAsync(message.Data, token);
            }

            await Task.Delay(1000, token);
        }
    }
}