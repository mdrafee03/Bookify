using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Abstractions.Caching;

public interface ICacheQuery<T> : IQuery<T>, ICacheQuery;

public interface ICacheQuery
{
    public string CacheKey { get; }
    public TimeSpan? Expiration { get; }
}
