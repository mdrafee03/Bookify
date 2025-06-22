using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration = null)
    {
        return expiration is null
            ? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            }
            : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
    }
}
