using AzureTelemetry.Infrastructure;
using AzureTelemetry.Infrastructure.Extensions;
using AzureTelemetry.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAzureServices();
builder.AddOpenTelemetry();

builder.Services.AddHealthChecks()
    .AddAzureBlobStorage(x => x.GetRequiredService<AzureService>().BlobServiceClient)
    .AddAzureTable(x => x.GetRequiredService<AzureService>().TableServiceClient);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();