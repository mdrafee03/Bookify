using Bookify.Application.Abstractions.Data;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bookify.Api.HealthCheck;

public class SqlHealthCheck(ISqlConnectionFactory sqlConnectionFactory): IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        try
        {
            await connection.ExecuteAsync("SELECT 1");
            return HealthCheckResult.Healthy("Database is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not reachable", ex);
        }
    }
}