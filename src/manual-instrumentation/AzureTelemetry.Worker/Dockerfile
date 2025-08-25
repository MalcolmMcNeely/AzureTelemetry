FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
WORKDIR /src
COPY ["AzureTelemetry.Worker/*.csproj", "AzureTelemetry.Worker/"]
COPY ["AzureTelemetry.Infrastructure/*.csproj", "AzureTelemetry.Infrastructure/"]
RUN dotnet restore "AzureTelemetry.Worker/AzureTelemetry.Worker.csproj"
COPY . .
RUN dotnet publish "AzureTelemetry.Worker/AzureTelemetry.Worker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureTelemetry.Worker.dll"]
