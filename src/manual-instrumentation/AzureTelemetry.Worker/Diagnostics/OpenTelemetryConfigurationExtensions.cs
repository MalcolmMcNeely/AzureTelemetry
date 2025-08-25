using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AzureTelemetry.Worker.Diagnostics;

public static class OpenTelemetryConfigurationExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
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
                    .AddSource(Defaults.Telemetry.ServiceName)
                    .AddSource("Azure.*")
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://otel-collector:4317"))
                    .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://otel-collector:4317"));
            });

        return builder;
    }
}