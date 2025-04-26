using System.Text.Json;
using Bookify.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private readonly IDistributedCache _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
        return cachedValue is null ? default : JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var serialized = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(
            key,
            serialized,
            CacheOptions.Create(expiration),
            cancellationToken
        );
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }
}
