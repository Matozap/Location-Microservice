using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LocationService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Data.Context;

public static class LocationSeedData
{
    private record RawCity(int id, string name);
    private record RawState(int id, string name, string state_code, RawCity[] cities);
    private record RawCountry(int id, string name, string iso2, RawState[] states);
    
    public static void SeedAllCountriesData(LocationContext context)
    {
        try
        {
            context.Database.EnsureCreated();
            if (context.Countries.AsNoTracking().OrderBy(e => e.Id).FirstOrDefault() == null)
            {
                var seedFile = string.Concat(File.ReadAllLines(@"D:\Local\src\Matozap\Location-Microservice\InputData\countries-states-cities.json"));
                var countryList = JsonSerializer.Deserialize<List<RawCountry>>(seedFile);

                foreach (var country in countryList)
                {
                    context.Countries.Add(new Country
                    {
                        Id = country.iso2,
                        Name = country.name,
                        LastUpdateDate = DateTime.UtcNow,
                        LastUpdateUserId = "System",
                        Disabled = false
                    });
                    foreach (var state in country.states)
                    {
                        context.States.Add(new State
                        {
                            Id = state.id,
                            Code = state.state_code,
                            Name = state.name,
                            CountryId = country.iso2,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        });
                        foreach (var city in state.cities)
                        {
                            context.Cities.Add(new City
                            {
                                Id = city.id,
                                Name = city.name,
                                StateId = state.id,
                                LastUpdateDate = DateTime.UtcNow,
                                LastUpdateUserId = "System",
                                Disabled = false
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
