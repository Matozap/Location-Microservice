using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LocationService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Database.Context;

public static class DatabaseSeed
{
    private record RawCity(int Id, string Name);
    private record RawState(int Id, string Name, string state_code, RawCity[] Cities);
    private record RawCountry(string Name, string Iso2, RawState[] States);
    
    public static void SeedAllCountriesData(DatabaseContext context)
    {
        try
        {
            context.Database.EnsureCreated();
            if (context.Countries.AsNoTracking().OrderBy(e => e.Id).FirstOrDefault() == null)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Database","Context","Seed", "countries-states-cities.json");
                var seedFile = string.Concat(File.ReadAllLines(path));
                var countryList = JsonSerializer.Deserialize<List<RawCountry>>(seedFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                foreach (var country in countryList)
                {
                    context.Countries.Add(new Country
                    {
                        Id = country.Iso2,
                        Name = country.Name,
                        LastUpdateDate = DateTime.UtcNow,
                        LastUpdateUserId = "System",
                        Disabled = false
                    });
                    foreach (var state in country.States)
                    {
                        var createdEntity = context.States.Add(new State
                        {
                            Code = state.state_code,
                            Name = state.Name,
                            CountryId = country.Iso2,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        }).Entity;
                        foreach (var city in state.Cities)
                        {
                            context.Cities.Add(new City
                            {
                                Name = city.Name,
                                StateId = createdEntity.Id,
                                LastUpdateDate = DateTime.UtcNow,
                                LastUpdateUserId = "System",
                                Disabled = false,
                                State = createdEntity
                            });
                        }
                    }
                }

                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}  - {ex.StackTrace}");
        }
    }
}
