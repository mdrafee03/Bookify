using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Bookify.Api.HealthCheck;

public class RedisCacheHealthCheck : IHealthCheck
{
    private readonly RedisCacheOptions _options;
    private readonly ConnectionMultiplexer _connection;

    public RedisCacheHealthCheck(IOptions<RedisCacheOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
        _connection = ConnectionMultiplexer.Connect(GetConnectionOptions());
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return Task.FromResult(
            _connection.IsConnected
                ? HealthCheckResult.Healthy("Redis connection is working!")
                : HealthCheckResult.Unhealthy("Redis connection not working!")
        );
    }

    private ConfigurationOptions GetConnectionOptions()
    {
        ConfigurationOptions redisConnectionOptions =
            (_options.ConfigurationOptions != null)
                ? ConfigurationOptions.Parse(_options.ConfigurationOptions.ToString())
                : ConfigurationOptions.Parse(_options.Configuration);

        redisConnectionOptions.AbortOnConnectFail = false;

        return redisConnectionOptions;
    }
}
