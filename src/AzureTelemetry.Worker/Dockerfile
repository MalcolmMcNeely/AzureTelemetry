FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
WORKDIR /src
COPY ["AzureTelemetry.Worker/*.csproj", "AzureTelemetry.Worker/"]
COPY ["AzureTelemetry.Infrastructure/*.csproj", "AzureTelemetry.Infrastructure/"]
RUN dotnet restore "AzureTelemetry.Worker/AzureTelemetry.Worker.csproj"
COPY . .
RUN dotnet publish "AzureTelemetry.Worker/AzureTelemetry.Worker.csproj" -c Release -o /app/publish

# Add OpenTelemetry .NET Auto-Instrumentation
ARG OTEL_VERSION=1.7.0
RUN apt-get update && apt-get install -y unzip curl
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v${OTEL_VERSION}/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh
RUN apt-get update && apt-get install -y unzip && \
    OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto" sh otel-dotnet-auto-install.sh
RUN chmod +x /otel-dotnet-auto/instrument.sh

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /
COPY --from=publish /otel-dotnet-auto ./otel-dotnet-auto
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/bin/bash", "-c", "source /otel-dotnet-auto/instrument.sh && dotnet AzureTelemetry.Worker.dll"]
