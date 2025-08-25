using AzureTelemetry.Infrastructure;
using AzureTelemetry.Infrastructure.Extensions;
using AzureTelemetry.Worker;
using AzureTelemetry.Worker.Diagnostics;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.AddAzureServices();
builder.AddOpenTelemetry();

builder.Services.AddHealthChecks()
    .AddAzureBlobStorage(x => x.GetRequiredService<AzureService>().BlobServiceClient)
    .AddAzureTable(x => x.GetRequiredService<AzureService>().TableServiceClient);

builder.Services.AddHostedService<Worker>();

// 👇 Add minimal web server just for metrics/health
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Map /metrics endpoint
app.MapHealthChecks("/healthz");

app.Run();