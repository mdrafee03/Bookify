using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

public class QueryCachingBehavior<TRequest, TResponse>(ICacheService cache,  ILogger<QueryCachingBehavior<TRequest, TResponse>> logger): IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheQuery where TResponse : Result
{
    private readonly ICacheService _cache = cache;
    private readonly ILogger<QueryCachingBehavior<TRequest, TResponse>> _logger = logger;
        
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CacheKey))
        {
            return await next();
        }

        var cacheResult = await _cache.GetAsync<TResponse>(request.CacheKey, cancellationToken);
        
        if (cacheResult is not null)
        {
           _logger.LogDebug("Cache hit: {CacheKey}", request.CacheKey); 
            return cacheResult;
        }
        var result = await next();
        
        if (result.IsSuccess)
        {
            await _cache.SetAsync(request.CacheKey, result, request.Expiration, cancellationToken);
        }
        else
        {
            await _cache.RemoveAsync(request.CacheKey, cancellationToken);
        }
        
        return result;
    }
}