using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LocationService.Domain;
using LocationService.Infrastructure.Extensions;
using LocationService.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Database.Context;

public static class DatabaseSeed
{
    private record RawCity(string Name);
    private record RawState(string Code, string Name, RawCity[] Cities);
    private record RawCountry(string Code, string Name, string Currency, string CurrencyName, string Region, string SubRegion, RawState[] States);

    public static async Task SeedAllCountriesDataAsync(DatabaseContext context)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            if (context.Countries.AsNoTracking().OrderBy(e => e.Id).FirstOrDefault() == null)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Database","Context","Seed", "seed.json");
                var seedFile = string.Concat(await File.ReadAllLinesAsync(path));
                var countryList = seedFile.Deserialize<List<RawCountry>>();

                context.ChangeTracker.AutoDetectChangesEnabled = false;
                var countries = new List<Country>(countryList.Count);
                var states = new List<State>();
                var cities = new List<City>();
                
                foreach (var country in countryList)
                {
                    var newCountry = new Country
                    {
                        Id = UniqueIdGenerator.GenerateSequentialId(),
                        Code = country.Code,
                        Name = country.Name,
                        Currency = country.Currency,
                        CurrencyName = country.CurrencyName,
                        Region = country.Region,
                        SubRegion = country.SubRegion,
                        LastUpdateDate = DateTime.UtcNow,
                        LastUpdateUserId = "System",
                        Disabled = false
                    };
                    countries.Add(newCountry);

                    foreach (var state in country.States)
                    {
                        var newState = new State
                        {
                            Id = UniqueIdGenerator.GenerateSequentialId(),
                            Code = state.Code,
                            Name = state.Name,
                            CountryId = newCountry.Id,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        };
                        states.Add(newState);

                        cities.AddRange(state.Cities.Select(city => new City
                        {
                            Id = UniqueIdGenerator.GenerateSequentialId(),
                            Name = city.Name,
                            StateId = newState.Id,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        }));
                    }
                }

                await context.Countries.AddRangeAsync(countries);
                await context.States.AddRangeAsync(states);
                await context.Cities.AddRangeAsync(cities);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}  - {ex.StackTrace}");
        }
    }
}
