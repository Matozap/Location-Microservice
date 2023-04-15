using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Infrastructure.Bus;
using LocationService.Infrastructure.Database.Context;
using LocationService.Infrastructure.Extensions;
using LocationService.Infrastructure.Utils;
using LocationService.Message.Events;
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

    public async Task<T> AddAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        entity.Id = UniqueIdGenerator.GenerateSequentialId();
        await CreateOutboxMessage(entity, nameof(AddAsync));
        await _applicationContext.AddAsync(entity);
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }

    public async Task<T> UpdateAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);

        _applicationContext.Update(entity);
        await CreateOutboxMessage(entity, nameof(UpdateAsync));
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    public async Task<T> DeleteAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        _applicationContext.Remove(entity);
        await CreateOutboxMessage(entity, nameof(DeleteAsync));
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    public async Task<List<T>> GetAsListAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending = null,
        Expression<Func<T, TKey>> orderDescending = null, Expression<Func<T, T>> selectExpression = null, bool includeNavigationalProperties = false) where T: EntityBase
    {
        var result = ApplySpec(predicate, orderAscending, orderDescending, selectExpression);
        return includeNavigationalProperties switch
        {
            true when typeof(T) == typeof(Country) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<Country>)).ToList() as List<T>,
            true when typeof(T) == typeof(State) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<State>)).ToList() as List<T>,
            true when typeof(T) == typeof(City) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<City>)).ToList() as List<T>,
            _ => await result.ToListAsync()
        };
    }
    
    public async Task<T> GetAsSingleAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending = null,
        Expression<Func<T, TKey>> orderDescending = null, Expression<Func<T, T>> selectExpression = null, bool includeNavigationalProperties = false) where T: EntityBase
    {
        var result = ApplySpec(predicate, orderAscending, orderDescending, selectExpression);

        return includeNavigationalProperties switch
        {
            true when typeof(T) == typeof(Country) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<Country>)).FirstOrDefault() as T,
            true when typeof(T) == typeof(State) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<State>)).FirstOrDefault() as T,
            true when typeof(T) == typeof(City) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<City>)).FirstOrDefault() as T,
            _ => await result.FirstOrDefaultAsync()
        };
    }
    
    private IQueryable<T> ApplySpec<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending,
        Expression<Func<T, TKey>> orderDescending, Expression<Func<T, T>> selectExpression) where T: EntityBase
    {
        var result = _applicationContext.Set<T>().AsQueryable();

        if (predicate != null)
        {
            result = result.Where(predicate);
        }

        if (orderAscending != null)
        {
            result = result.OrderBy(orderAscending);
        }
        
        if (orderDescending != null)
        {
            result = result.OrderBy(orderDescending);
        }
        
        if (orderDescending != null)
        {
            result = result.Select(selectExpression);
        }

        return result;
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
                    country.States = _applicationContext.Set<State>().Where(state => state.CountryId == country.Id).OrderBy(state => state.Name)
                        .AsNoTracking()
                        .ToList();

                    country.States = country.States.Select(f =>
                    {
                        f.Cities = _applicationContext.Set<City>().Where(city => city.StateId == f.Id).OrderBy(city => city.Name)
                            .AsNoTracking()
                            .ToList();
                        return f;
                    }).ToList();

                    return country;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(country => country.States.Where(state => !state.Disabled).OrderBy(state => state.Name))
                    .ThenInclude(s => s.Cities.Where(city => !city.Disabled).OrderBy(city => city.Name))
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
                    state.Cities = _applicationContext.Set<City>().Where(city => city.StateId == state.Id).OrderBy(city => city.Name)
                        .AsNoTracking()
                        .ToList();
                    return state;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(state => state.Cities.Where(c => !c.Disabled).OrderBy(city => city.Name))
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
    
    private async Task CreateOutboxMessage<T>(T entity, string sourceMethod) where T: EntityBase
    {
        if(typeof(T) == typeof(Outbox)) return;
        
        var outbox = new Outbox
        {
            Id = UniqueIdGenerator.GenerateSequentialId(),
            JsonObject = entity.Serialize(),
            LastUpdateDate = entity.LastUpdateDate,
            LastUpdateUserId = entity.LastUpdateUserId,
            ObjectType = typeof(T).Name,
            Operation = sourceMethod switch
            {
                nameof(AddAsync) => Operation.Create,
                nameof(UpdateAsync) => Operation.Update,
                nameof(DeleteAsync) => Operation.Delete,
                _ => Operation.None
            }
        };

        await _applicationContext.AddAsync(outbox);
    }
}
