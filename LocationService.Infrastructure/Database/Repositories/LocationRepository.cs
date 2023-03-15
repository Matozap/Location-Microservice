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

public class LocationRepository : ILocationRepository
{
    private readonly DatabaseContext _applicationContext;
    private readonly DatabaseOptions _databaseOptions;

    public LocationRepository(DatabaseContext applicationContext, DatabaseOptions databaseOptions)
    {
        _applicationContext = applicationContext;
        _databaseOptions = databaseOptions;
    }

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        var result = _applicationContext.Set<Country>()
            .Where(e => !e.Disabled)
            .OrderBy(e => e.Name)
            .Select(e => new Country
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                Currency = e.Currency,
                CurrencyName = e.CurrencyName,
                Region = e.Region,
                SubRegion = e.SubRegion
            });
        return await result.ToListAsync();
    }

    public async Task<List<State>> GetAllStatesAsync(string countryId)
    {
        var parentByCode = await _applicationContext.Set<Country>().FirstOrDefaultAsync(e => e.Code == countryId) ?? new Country();
        var result = _applicationContext.Set<State>()
            .Where(s => s.CountryId == countryId || s.CountryId == parentByCode.Id)
            .Where(e => !e.Disabled)
            .OrderBy(e => e.Name);
            
        return await LoadAllNavigationalPropertiesAsync(result);
    }

    public async Task<List<City>> GetAllCitiesAsync(string stateId)
    {
        var parentByCode = await _applicationContext.Set<State>().FirstOrDefaultAsync(e => e.Code == stateId) ?? new State();
        var result = _applicationContext.Set<City>()
            .Where(s => s.StateId == stateId || s.StateId == parentByCode.Id)
            .Where(e => !e.Disabled)
            .OrderBy(e => e.Name);
        
        return await LoadAllNavigationalPropertiesAsync(result);
    }

    public async Task<Country> GetCountryAsync(Expression<Func<Country, bool>> predicate)
    {
        var result = _applicationContext.Set<Country>()
            .Where(e => !e.Disabled)
            .Where(predicate)
            .OrderBy(e => e.Name);;

        return (await LoadAllNavigationalPropertiesAsync(result)).FirstOrDefault();
    }

    public async Task<State> GetStateAsync(Expression<Func<State, bool>> predicate)
    {
        var result = _applicationContext.Set<State>()
            .Where(e => !e.Disabled)
            .Where(predicate)
            .OrderBy(e => e.Name);

        return (await LoadAllNavigationalPropertiesAsync(result)).FirstOrDefault();
    }

    public async Task<City> GetCityAsync(Expression<Func<City, bool>> predicate)
    {
        var result = _applicationContext.Set<City>()
            .Where(e => !e.Disabled)
            .Where(predicate)
            .OrderBy(e => e.Name);;

        return (await LoadAllNavigationalPropertiesAsync(result)).FirstOrDefault();
    }

    public async Task<Country> AddAsync(Country entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.Id = UniqueIdGenerator.GenerateSequentialId();
        return await AddAsync<Country>(entity);
    }
    
    public async Task<State> AddAsync(State entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.Id = UniqueIdGenerator.GenerateSequentialId();
        return await AddAsync<State>(entity);
    }
    
    public async Task<City> AddAsync(City entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.Id = UniqueIdGenerator.GenerateSequentialId();
        return await AddAsync<City>(entity);
    }

    private async Task<T> AddAsync<T>(T entity) where T: class
    {
        await _applicationContext.AddAsync(entity);
        await _applicationContext.SaveChangesAsync();

        return entity;
    }
    
    public async Task<T> UpdateAsync<T>(T entity) where T: class
    {
        ArgumentNullException.ThrowIfNull(entity);
            
        _applicationContext.Update(entity);
        await _applicationContext.SaveChangesAsync();

        return entity;
    }
    
    public async Task<T> DeleteAsync<T>(T entity) where T: class
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
                return partialResult.Select(e =>
                {
                    e.States = _applicationContext.Set<State>().Where(s => s.CountryId == e.Id).AsNoTracking().ToList();
                    return e;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(c => c.States.Where(s => !s.Disabled))
                    .ThenInclude(s => s.Cities.Where(c => !c.Disabled))
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
                return partialResult.Select(e =>
                {
                    e.Cities = _applicationContext.Set<City>().Where(s => s.StateId == e.Id).AsNoTracking().ToList();
                    return e;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(s => s.Cities.Where(c => !c.Disabled))
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();
        }
    }
    
    private async Task<List<City>> LoadAllNavigationalPropertiesAsync(IQueryable<City> source)
    {
        return _databaseOptions.EngineType switch
        {
            EngineType.NonRelational => await source.AsNoTracking().ToListAsync(),
            _ => source.AsSplitQuery().AsNoTracking().ToList()
        };
    }
}
