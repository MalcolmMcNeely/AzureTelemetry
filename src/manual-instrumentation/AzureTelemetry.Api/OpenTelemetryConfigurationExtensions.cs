using AzureTelemetry.Worker;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AzureTelemetry;

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
                    .AddSource(Defaults.Telemetry.ActivitySourceName)
                    .AddSource("Azure.*")
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317"))
                    .AddConsoleExporter();
            });

        return builder;
    }
}