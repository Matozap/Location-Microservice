using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Data.Repository;

public class LocationRepository : ILocationRepository
{
    private readonly LocationContext _applicationContext;

    public LocationRepository(LocationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        var result = _applicationContext.Set<Country>()
            .Where(e => !e.Disabled)
            .OrderBy(e => e.Name)
            .Select(e => new Country()
            {
                Id = e.Id,
                Name = e.Name
            });
        return await result.ToListAsync();
    }

    public async Task<List<State>> GetAllStatesAsync(string countryId)
    {
        var result = _applicationContext.Set<State>()
            .Where(s => s.CountryId == countryId)
            .Where(e => !e.Disabled)
            .OrderBy(e => e.Name);
            
        return await LoadAllNavigationalProperties(result).ToListAsync();
    }

    public async Task<List<City>> GetAllCitiesAsync(int stateId)
    {
        var result = _applicationContext.Set<City>()
            .Where(s => s.StateId == stateId)
            .Where(e => !e.Disabled)
            .OrderBy(e => e.Name);
        
        return await LoadAllNavigationalProperties(result).ToListAsync();
    }

    public async Task<Country> GetCountryAsync(Expression<Func<Country, bool>> predicate)
    {
        var result = _applicationContext.Set<Country>()
            .Where(e => !e.Disabled)
            .Where(predicate)
            .OrderBy(e => e.Name);;

        return await LoadAllNavigationalProperties(result).FirstOrDefaultAsync();
    }

    public async Task<State> GetStateAsync(Expression<Func<State, bool>> predicate)
    {
        var result = _applicationContext.Set<State>()
            .Where(e => !e.Disabled)
            .Where(predicate)
            .OrderBy(e => e.Name);

        return await LoadAllNavigationalProperties(result).FirstOrDefaultAsync();
    }

    public async Task<City> GetCityAsync(Expression<Func<City, bool>> predicate)
    {
        var result = _applicationContext.Set<City>()
            .Where(e => !e.Disabled)
            .Where(predicate)
            .OrderBy(e => e.Name);;

        return await LoadAllNavigationalProperties(result).FirstOrDefaultAsync();
    }
   
    public async Task<T> AddAsync<T>(T entity) where T: class
    {
        ArgumentNullException.ThrowIfNull(entity);
            
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
    
    private static IQueryable<Country> LoadAllNavigationalProperties(IQueryable<Country> source)
    {
        return source.Include(c => c.States.Where(s => !s.Disabled)).ThenInclude(s => s.Cities.Where(c => !c.Disabled)).AsNoTracking();
    }
    private static IQueryable<State> LoadAllNavigationalProperties(IQueryable<State> source)
    {
        return source.Include(s => s.Cities.Where(c => !c.Disabled)).AsNoTracking();
    }
    private static IQueryable<City> LoadAllNavigationalProperties(IQueryable<City> source)
    {
        return source.AsNoTracking();
    }
}
