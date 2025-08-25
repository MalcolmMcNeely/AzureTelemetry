using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AzureTelemetry.Worker;

public static class OpenTelemetryConfigurationExtensions
{
    public static HostApplicationBuilder AddOpenTelemetry(this HostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService("AzureTelemetry.Worker")
                    .AddAttributes(
                    [
                        new KeyValuePair<string, object>("service.version", "1.0.0")
                    ]);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(Defaults.Telemetry.ActivitySourceName)
                    .AddSource("Azure.*")
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317"))
                    .AddConsoleExporter();
            });

        return builder;
    }
}