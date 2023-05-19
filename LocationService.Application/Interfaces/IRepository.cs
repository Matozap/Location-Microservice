using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Domain;

namespace LocationService.Application.Interfaces;

public interface IRepository
{
    Task<List<T>> GetAsListAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending = null,
        Expression<Func<T, TKey>> orderDescending = null, Expression<Func<T, T>> selectExpression = null, bool includeNavigationalProperties = false) where T: EntityBase;
    Task<T> GetAsSingleAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending = null,
        Expression<Func<T, TKey>> orderDescending = null, Expression<Func<T, T>> selectExpression = null, bool includeNavigationalProperties = false) where T: EntityBase;
    Task<T> AddAsync<T>(T entity) where T : EntityBase;
    Task<T> UpdateAsync<T>(T entity) where T : EntityBase;
    Task<T> DeleteAsync<T>(T entity) where T : EntityBase;
}
