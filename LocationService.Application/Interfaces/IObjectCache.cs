using System.Threading.Tasks;

namespace LocationService.Application.Interfaces;

public interface IObjectCache
{
    public Task<T> GetCacheValueAsync<T>(string key, System.Threading.CancellationToken token = default) where T : class;
    public Task SetCacheValueAsync<T>(string key, T value, System.Threading.CancellationToken token = default) where T : class;
    public Task RemoveValueAsync(string key, System.Threading.CancellationToken token = default);
}
