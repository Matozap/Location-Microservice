using System;
using LocationService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Database.Context;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger)
        : base(options)
    {
        DatabaseSeed.SeedAllCountriesData(this);
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
        CreateCountryModel(modelBuilder);
        CreateStateModel(modelBuilder);
        CreateCityModel(modelBuilder);
    }
    
    private void CreateCountryModel(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Country>(entity =>
        {
            modelBuilder.HasDefaultContainer("Country");
            entity.HasKey(e => e.Id);
            entity.HasPartitionKey(e => e.Id);
            entity.Property(p => p.Id).ToJsonProperty("id").IsRequired();
            entity.Property(p => p.Name).IsRequired();
            entity.HasMany(c => c.States);
        });

    private void CreateStateModel(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code);
            entity.Property(p => p.Id).ToJsonProperty("id").IsRequired();
            entity.Property(p => p.Name).IsRequired();
            entity.HasMany(s => s.Cities).WithOne(c => c.State);
        });

    private void CreateCityModel(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(p => p.Id).ToJsonProperty("id").IsRequired();
            entity.Property(p => p.Name).IsRequired();
            entity.HasOne(c => c.State).WithMany(s => s.Cities);
        });
}
