using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Caching;

public sealed class Cache : ICache
{
    private static readonly string Prefix = Environment.GetEnvironmentVariable("SERVICE_NAME");
    private int _currentErrorCount;
    private bool _automaticallyDisabled;
    private DateTime _automaticallyDisabledTime;
    private readonly List<string> _errorMessages;
    private readonly IDistributedCache _distributedCache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<Cache> _logger;
    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
    
    private const string AllKeys = "ICacheAllKeys";

    public Cache(IDistributedCache cache, CacheOptions cacheOptions, ILogger<Cache> logger)
    {
        _distributedCache = cache;
        _cacheOptions = cacheOptions;
        _logger = logger;
        _distributedCacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
            SlidingExpiration = TimeSpan.FromSeconds(30)
        };
        _errorMessages = new List<string>();
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
            var deserializedObj = result.Deserialize<T>();
            SetHealthyStatus();
            return deserializedObj;
        }
        catch (Exception ex)
        {
            _errorMessages.Add(ex.Message);
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

            var result = value.Serialize();
            await _distributedCache.SetStringAsync(cacheKey, result, _distributedCacheEntryOptions, token);
            _ = AddToKeyListAsync(cacheKey, token);
            SetHealthyStatus();
        }
        catch (Exception ex)
        {
            _errorMessages.Add(ex.Message);
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
            _errorMessages.Add(ex.Message);
            _logger.LogDebug("Could not remove key {CacheKey} from cache - {Error}", key, ex.Message);
            SetUnhealthyStatus();
        }
    }

    public async Task ClearCacheAsync(CancellationToken token = default)
    {
        var result = await _distributedCache.GetStringAsync(AllKeys, token);
        if (result != null)
        {
            var keys = result.Deserialize<List<string>>();
            foreach (var key in keys)
            {
                await RemoveValueAsync(key, token);
            }
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
            var errors = string.Join(", ", _errorMessages.Distinct().ToList());
            _logger.LogWarning("Cache was switched to disabled after reaching the max number of consecutive errors allowed ({ErrorsAllowed}) - Errors Found: {Errors}"
                , _cacheOptions.HealthCheck.MaxErrorsAllowed.ToString(), errors);
            _errorMessages.Clear();
        }
    }
    
    private void SetHealthyStatus()
    {
        _currentErrorCount = 0;
        _cacheOptions.Disabled = false;
        _automaticallyDisabled = false;
    }

    private async Task AddToKeyListAsync(string key, CancellationToken token = default)
    {
        var keys = new List<string>();
        var result = await _distributedCache.GetStringAsync(AllKeys, token);
        if (result != null)
        {
            keys = result.Deserialize<List<string>>();
        }

        if (!keys.Contains(key))
        {
            keys.Add(key);
            await _distributedCache.SetStringAsync(AllKeys, keys.Serialize(), new DistributedCacheEntryOptions(), token);
        }
    }
}
