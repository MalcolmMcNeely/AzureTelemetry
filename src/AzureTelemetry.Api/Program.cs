using AzureTelemetry.Infrastructure.Extensions;
using AzureTelemetry.Infrastructure.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddAzureServices();

builder.Services.AddOpenApi();

var app = builder.Build();

{ // normally guarded with if(app.Environment.IsDevelopment())
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Azure Service Demo")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.MapPost("/data", async (string data, bool sendToWorker, IDataRepository repo, IQueueService queueService, CancellationToken token) =>
{
    var id = await repo.StoreAsync(data, token);

    if (sendToWorker)
    {
        await queueService.Send(new MessageForWorker(data, Guid.NewGuid().ToString()), token);
    }

    return Results.Created($"/data/{id}", new { id });
});

app.MapGet("/data/{id}", async (string id, IDataRepository repo, CancellationToken token) =>
{
    try
    {
        var data = await repo.RetrieveAsync(id, token);
        return Results.Ok(new { id, data });
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.Run();