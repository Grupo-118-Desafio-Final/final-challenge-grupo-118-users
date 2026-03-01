using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Cache;

public class CacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan expiration)
    {
        var cached = await _cache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<T>(cached);

        var result = await factory();

        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });

        return result;
    }
}
