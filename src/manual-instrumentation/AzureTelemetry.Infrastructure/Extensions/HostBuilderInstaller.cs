using AzureTelemetry.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureTelemetry.Infrastructure.Extensions;

public static class HostBuilderInstaller
{
    public static void AddAzureServices(this IHostApplicationBuilder builder, string connectionString = Defaults.Azure.AzuriteConnectionString)
    {
        builder.Services.AddSingleton(new AzureService(connectionString));
        builder.Services.AddScoped<IQueueService, QueueService>();
        builder.Services.AddScoped<IDataRepository, DataRepository>();
    }
}