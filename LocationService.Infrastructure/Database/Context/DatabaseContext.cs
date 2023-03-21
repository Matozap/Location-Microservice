using System;
using LocationService.Domain;
using LocationService.Infrastructure.Database.Context.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Database.Context;

public sealed class DatabaseContext : DbContext
{
    private readonly ILogger<DatabaseContext> _logger;
    private readonly DatabaseOptions _databaseOptions;

    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger, DatabaseOptions databaseOptions)
        : base(options)
    {
        _logger = logger;
        _databaseOptions = databaseOptions;
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<City> Cities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.LogInformation("Using {DatabaseType} database ({DatabaseEngine})", _databaseOptions.DatabaseType, Enum.GetName(typeof(EngineType), _databaseOptions.EngineType));
        CreateCountryModel(modelBuilder);
        CreateStateModel(modelBuilder);
        CreateCityModel(modelBuilder);
    }
    
    private void CreateCountryModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>().ToContainer("Country");
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(p => p.Id).ToJsonProperty("id").IsRequired();
            entity.Property(p => p.Code).IsRequired();
            entity.HasIndex(e => e.Code);
            entity.Property(p => p.Name).IsRequired();
            entity.HasMany(c => c.States);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.Id);
        });
    }

    private void CreateStateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<State>().ToContainer("State");
        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ToJsonProperty("id").IsRequired();
            entity.Property(e => e.Code).IsRequired();
            entity.HasIndex(e => e.Code);
            entity.Property(e => e.Name).IsRequired();
            entity.HasMany(s => s.Cities).WithOne(c => c.State);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.CountryId);
            entity.HasManualThroughput(10000);
        });
    }

    private void CreateCityModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().ToContainer("City");
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(p => p.Id).ToJsonProperty("id").IsRequired();
            entity.Property(p => p.Name).IsRequired();
            entity.HasIndex(e => e.Name);
            entity.HasOne(c => c.State).WithMany(s => s.Cities);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.StateId);
            entity.HasManualThroughput(10000);
        });
    }

    public void SeedData() => DatabaseSeed.SeedAllCountriesDataAsync(this).Wait();
}
