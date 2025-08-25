FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
WORKDIR /src
COPY ["AzureTelemetry.Api/*.csproj", "AzureTelemetry.Api/"]
COPY ["AzureTelemetry.Infrastructure/*.csproj", "AzureTelemetry.Infrastructure/"]
RUN dotnet restore "AzureTelemetry.Api/AzureTelemetry.Api.csproj"
COPY . .
RUN dotnet publish "AzureTelemetry.Api/AzureTelemetry.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .    
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "AzureTelemetry.Api.dll"]
