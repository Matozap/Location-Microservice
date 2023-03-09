namespace LocationService.Infrastructure.Database.Context;

public class DatabaseOptions
{
    public string InstanceName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseType { get; set; }
}