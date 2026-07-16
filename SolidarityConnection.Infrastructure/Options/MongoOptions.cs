namespace SolidarityConnection.Infrastructure.Options;

public sealed class MongoOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string CollectionName { get; init; } = string.Empty;
}