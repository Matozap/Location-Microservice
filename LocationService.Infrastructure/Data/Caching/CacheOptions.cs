namespace LocationService.Infrastructure.Data.Caching;

public class CacheOptions
{
    public record HealthCheckOptions(bool Enabled, int MaxErrorsAllowed, int ResetIntervalMinutes);
    public string CacheType { get; set; }
    public string InstanceName { get; set; }
    public string ConnectionString { get; set; }
    public HealthCheckOptions HealthCheck { get; set; }
    public bool Disabled { get; set; }
}


