namespace AzureTelemetry.Infrastructure;

public static class Defaults
{
    public static class Azure
    {
        public const string AzuriteConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
    }

    public static class Tables
    {
        public const string DataLookup = "lookup";
    }

    public static class Containers
    {
        public const string Data = "data";
    }

    public static class Queues
    {
        public const string MessageTransport = "message-transport";
    }
}