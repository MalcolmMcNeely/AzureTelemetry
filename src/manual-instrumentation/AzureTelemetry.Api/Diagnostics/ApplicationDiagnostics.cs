using System.Diagnostics.Metrics;

namespace AzureTelemetry.Api.Diagnostics;

public static class ApplicationDiagnostics
{
    public static readonly Meter Meter = new(Defaults.Telemetry.ServiceName);
    
    public static readonly Counter<long> DataCreatedCounter = Meter.CreateCounter<long>("data.created");
}