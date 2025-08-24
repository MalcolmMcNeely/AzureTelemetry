using AzureTelemetry.Infrastructure.Extensions;
using AzureTelemetry.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAzureServices();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();