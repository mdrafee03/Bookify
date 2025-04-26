namespace Bookify.Application.Abstractions.Caching;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    );
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
