using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Infrastructure.Database.Context;
using LocationService.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Database.Repositories;

public class EfRepository : IRepository
{
    private readonly DatabaseContext _applicationContext;
    private readonly DatabaseOptions _databaseOptions;

    public EfRepository(DatabaseContext applicationContext, DatabaseOptions databaseOptions)
    {
        _applicationContext = applicationContext;
        _databaseOptions = databaseOptions;
    }

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        var result = _applicationContext.Set<Country>()
            .Where(country => !country.Disabled)
            .OrderBy(country => country.Name)
            .Select(country => new Country
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name,
                Currency = country.Currency,
                CurrencyName = country.CurrencyName,
                Region = country.Region,
                SubRegion = country.SubRegion
            });
        return await result.ToListAsync();
    }

    public async Task<List<State>> GetAllStatesAsync(string countryId)
    {
        var parentByCode = await _applicationContext.Set<Country>().FirstOrDefaultAsync(e => e.Code == countryId) ?? new Country();
        var result = _applicationContext.Set<State>()
            .Where(state => state.CountryId == countryId || state.CountryId == parentByCode.Id)
            .Where(state => !state.Disabled)
            .OrderBy(state => state.Name);
            
        return await LoadAllNavigationalPropertiesAsync(result);
    }

    public async Task<List<City>> GetAllCitiesAsync(string stateId)
    {
        var parentByCode = await _applicationContext.Set<State>().FirstOrDefaultAsync(e => e.Code == stateId) ?? new State();
        var result = _applicationContext.Set<City>()
            .Where(city => city.StateId == stateId || city.StateId == parentByCode.Id)
            .Where(city => !city.Disabled)
            .OrderBy(city => city.Name);
        
        return await LoadAllNavigationalPropertiesAsync(result);
    }

    public async Task<Country> GetCountryAsync(Expression<Func<Country, bool>> predicate)
    {
        var result = _applicationContext.Set<Country>()
            .Where(country => !country.Disabled)
            .Where(predicate)
            .OrderBy(country => country.Name);

        return (await LoadAllNavigationalPropertiesAsync(result)).FirstOrDefault();
    }

    public async Task<State> GetStateAsync(Expression<Func<State, bool>> predicate)
    {
        var result = _applicationContext.Set<State>()
            .Where(state => !state.Disabled)
            .Where(predicate)
            .OrderBy(state => state.Name);

        return (await LoadAllNavigationalPropertiesAsync(result)).FirstOrDefault();
    }

    public async Task<City> GetCityAsync(Expression<Func<City, bool>> predicate)
    {
        var result = _applicationContext.Set<City>()
            .Where(city => !city.Disabled)
            .Where(predicate)
            .OrderBy(city => city.Name);

        return (await LoadAllNavigationalPropertiesAsync(result)).FirstOrDefault();
    }

    public async Task<T> AddAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.Id = UniqueIdGenerator.GenerateSequentialId();
        await _applicationContext.AddAsync(entity);
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    public async Task<T> UpdateAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
            
        _applicationContext.Update(entity);
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    public async Task<T> DeleteAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
            
        _applicationContext.Remove(entity);
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    private async Task<List<Country>> LoadAllNavigationalPropertiesAsync(IQueryable<Country> source)
    {
        switch (_databaseOptions.EngineType)
        {
            case EngineType.NonRelational:
            {
                var partialResult = await source.AsNoTracking().ToListAsync();
                return partialResult.Select(country =>
                {
                    country.States = _applicationContext.Set<State>().Where(state => state.CountryId == country.Id)
                        .AsNoTracking()
                        .ToList();

                    country.States = country.States.Select(f =>
                    {
                        f.Cities = _applicationContext.Set<City>().Where(city => city.StateId == f.Id)
                            .AsNoTracking()
                            .ToList();
                        return f;
                    }).ToList();

                    return country;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(country => country.States.Where(state => !state.Disabled))
                    .ThenInclude(s => s.Cities.Where(city => !city.Disabled))
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();
        }
    }
    
    private async Task<List<State>> LoadAllNavigationalPropertiesAsync(IQueryable<State> source)
    {
        switch (_databaseOptions.EngineType)
        {
            case EngineType.NonRelational:
            {
                var partialResult = await source.AsNoTracking().ToListAsync();
                return partialResult.Select(state =>
                {
                    state.Cities = _applicationContext.Set<City>().Where(city => city.StateId == state.Id)
                        .AsNoTracking()
                        .ToList();
                    return state;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(state => state.Cities.Where(c => !c.Disabled))
                    .Include(state => state.Country)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();
        }
    }
    
    private async Task<List<City>> LoadAllNavigationalPropertiesAsync(IQueryable<City> source)
    {
        switch (_databaseOptions.EngineType)
        {
            case EngineType.NonRelational:
                return await source.AsNoTracking().ToListAsync();
            default:
                return source.AsSplitQuery().Include(city => city.State).AsNoTracking().ToList();
        }
    }
}
