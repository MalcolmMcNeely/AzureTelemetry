using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AzureTelemetry.Api.Diagnostics;

public static class OpenTelemetryConfigurationExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService("AzureTelemetry.Api")
                    .AddAttributes(
                    [
                        new KeyValuePair<string, object>("service.version", "1.0.0")
                    ]);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(Defaults.Telemetry.ServiceName)
                    .AddSource("Azure.*")
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://otel-collector:4317"))
                    .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    // Provided by Asp.Net
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    // Process & runtime
                    .AddRuntimeInstrumentation()
                    // Custom meter
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddConsoleExporter()
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://otel-collector:4317"));
            });

        return builder;
    }
}