namespace LocationService.Infrastructure.Database.Context;

public class DatabaseOptions
{
    public string InstanceName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseType { get; set; }
    public bool SeedData { get; set; }
    public EngineType EngineType => DatabaseType == "Cosmos" ? EngineType.NonRelational : EngineType.Relational;
}

public enum EngineType
{
    Relational,
    NonRelational
}