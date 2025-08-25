using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AzureTelemetry.Worker.Diagnostics;

public static class ApplicationDiagnostics
{
    public static readonly Meter Meter = new(Defaults.Telemetry.ServiceName);
    
    public static readonly Counter<long> MessageSeenCounter = Meter.CreateCounter<long>("message.seen");

    public static readonly ActivitySource ActivitySource = new(Defaults.Telemetry.ServiceName);
}