using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Caching;

public sealed class Cache : ICache
{
    private static readonly string Prefix = Environment.GetEnvironmentVariable("SERVICE_NAME");
    private int _currentErrorCount;
    private bool _automaticallyDisabled;
    private DateTime _automaticallyDisabledTime;
    private readonly IDistributedCache _distributedCache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<Cache> _logger;
    
    public Cache(IDistributedCache cache, CacheOptions cacheOptions, ILogger<Cache> logger)
    {
        _distributedCache = cache;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }

    public async Task<T> GetCacheValueAsync<T>(string key, CancellationToken token = default) where T : class
    {
        try
        {
            CheckHealthStatus();
            
            if (_cacheOptions.Disabled)
                return null;
            
            var cacheKey = $"{Prefix}:{key}";
            var result = await _distributedCache.GetStringAsync(cacheKey, token);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            var deserializedObj = JsonSerializer.Deserialize<T>(result);
            SetHealthyStatus();
            return deserializedObj;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not get cache key {CacheKey} - {Error}",key, ex.Message);
            SetUnhealthyStatus();
            return null;
        }
    }
                
    public async Task SetCacheValueAsync<T>(string key, T value, CancellationToken token = default) where T : class
    {
        try
        {
            CheckHealthStatus();
            
            if (_cacheOptions.Disabled)
                return;

            var cacheKey = $"{Prefix}:{key}";
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };

            var result = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(cacheKey, result, cacheEntryOptions, token);
            SetHealthyStatus();
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not set cache key {CacheKey} - {Error}",key, ex.Message);
            SetUnhealthyStatus();
        }
    }

    public async Task RemoveValueAsync(string key, CancellationToken token = default)
    {
        try
        {
            CheckHealthStatus();
            
            if (_cacheOptions.Disabled)
                return;
            
            var cacheKey = $"{Prefix}:{key}";
            await _distributedCache.RemoveAsync(cacheKey, token);
            SetHealthyStatus();
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not remove key {CacheKey} from cache - {Error}", key, ex.Message);
            SetUnhealthyStatus();
        }
    }

    private void CheckHealthStatus()
    {
        if (_automaticallyDisabled && _automaticallyDisabledTime.AddMinutes(_cacheOptions.HealthCheck.ResetIntervalMinutes) < DateTime.UtcNow)
        {
            SetHealthyStatus();
            _logger.LogInformation("Cache was restored to enabled after being disabled for {ResetInterval} minutes", _cacheOptions.HealthCheck.ResetIntervalMinutes.ToString());
        }
    }

    private void SetUnhealthyStatus()
    {
        if (_currentErrorCount++ >= _cacheOptions.HealthCheck.MaxErrorsAllowed && _cacheOptions.HealthCheck.Enabled)
        {
            _cacheOptions.Disabled = true;
            _automaticallyDisabled = true;
            _automaticallyDisabledTime = DateTime.UtcNow;
            _logger.LogWarning("Cache was switched to disabled after reaching the max number of consecutive errors allowed ({ErrorsAllowed})", _cacheOptions.HealthCheck.MaxErrorsAllowed.ToString());
        }
    }
    
    private void SetHealthyStatus()
    {
        _currentErrorCount = 0;
        _cacheOptions.Disabled = false;
        _automaticallyDisabled = false;
    }
}
