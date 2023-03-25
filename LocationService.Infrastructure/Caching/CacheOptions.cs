namespace LocationService.Infrastructure.Caching;

public class CacheOptions
{
    public record HealthCheckOptions(bool Enabled, int MaxErrorsAllowed, int ResetIntervalMinutes);
    public string CacheType { get; set; }
    public string InstanceName { get; set; }
    public string ConnectionString { get; set; }
    public HealthCheckOptions HealthCheck { get; set; }
    public bool Disabled { get; set; }
    public string SqlServerTableExistQuery => $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '{InstanceName}'";
    public string SqlServerTableCreateQuery => 
        @$"CREATE TABLE [dbo].[{InstanceName}](
            [Id] [nvarchar](449) NOT NULL,
            [Value] [varbinary](max) NOT NULL,
            [ExpiresAtTime] [datetimeoffset](7) NOT NULL,
            [SlidingExpirationInSeconds] [bigint] NULL,
            [AbsoluteExpiration] [datetimeoffset](7) NULL,
            PRIMARY KEY CLUSTERED ([Id] ASC))";
}


