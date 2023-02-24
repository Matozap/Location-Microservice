using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Domain;

namespace LocationService.Application.Interfaces;

public interface ILocationRepository
{
    Task<List<Country>> GetAllCountriesAsync();
    Task<List<State>> GetAllStatesAsync(string countryId);
    Task<List<City>> GetAllCitiesAsync(int stateId);
    Task<Country> GetCountryAsync(Expression<Func<Country, bool>> predicate);
    Task<State> GetStateAsync(Expression<Func<State, bool>> predicate);
    Task<City> GetCityAsync(Expression<Func<City, bool>> predicate);
    Task<T> AddAsync<T>(T entity) where T : class;
    Task<T> UpdateAsync<T>(T entity) where T : class;
    Task<T> DeleteAsync<T>(T entity) where T : class;
}
