using System.Threading;
using System.Threading.Tasks;

namespace LocationService.Application.Interfaces;

public interface ICache
{
    public Task<T> GetCacheValueAsync<T>(string key, CancellationToken token = default) where T : class;
    public Task SetCacheValueAsync<T>(string key, T value, CancellationToken token = default) where T : class;
    public Task RemoveValueAsync(string key, CancellationToken token = default);
    Task ClearCacheAsync(CancellationToken token = default);
}
