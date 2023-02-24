using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Data.Cache;

public sealed class ObjectCache : IObjectCache
{
    private static readonly string Prefix = Environment.GetEnvironmentVariable("SERVICE_NAME");
    private readonly IDistributedCache _cache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<ObjectCache> _logger;
    
    public ObjectCache(IDistributedCache cache, CacheOptions cacheOptions, ILogger<ObjectCache> logger)
    {
        _cache = cache;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }

    public async Task<T> GetCacheValueAsync<T>(string key, CancellationToken token = default) where T : class
    {
        try
        {
            if (_cacheOptions.Disabled)
                return null;
            
            var cacheKey = $"{Prefix}:{key}";
            var result = await _cache.GetStringAsync(cacheKey, token);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            var deserializedObj = JsonSerializer.Deserialize<T>(result);
            return deserializedObj;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error loading cache: {Error}", ex.Message);
            return null;
        }
    }
                
    public async Task SetCacheValueAsync<T>(string key, T value, CancellationToken token = default) where T : class
    {
        try
        {
            if (_cacheOptions.Disabled)
                return;
            
            var cacheKey = $"{Prefix}:{key}";
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };

            var result = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(cacheKey, result, cacheEntryOptions, token);
        }
        catch { }
    }

    public async Task RemoveValueAsync(string key, CancellationToken token = default)
    {
        try
        {
            var cacheKey = $"{Prefix}:{key}";
            await _cache.RemoveAsync(cacheKey, token);
        }
        catch { }
    }
}
