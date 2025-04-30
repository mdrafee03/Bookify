using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(
    ILogger<ProcessOutboxMessagesJob> logger,
    ISqlConnectionFactory sqlConnectionFactory,
    IOptions<OutboxOptions> outboxOptions,
    IPublisher publisher,
    IDateTimeProvider dateTimeProvider
) : IJob
{
    private readonly ILogger<ProcessOutboxMessagesJob> _logger = logger;
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;
    private readonly IPublisher _publisher = publisher;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages...");

        using var connection = _sqlConnectionFactory.CreateConnection();
        var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessages(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    _jsonSerializerSettings
                )!;

                await _publisher.Publish(domainEvent, cancellationToken: context.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process outbox message");
                exception = e;
            }
            await UpdateOutboxMessages(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();
        _logger.LogInformation("Processed {Count} outbox messages", outboxMessages.Count);
    }

    private async Task UpdateOutboxMessages(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessage outboxMessage,
        Exception? exception
    )
    {
        const string sql = """
                UPDATE outbox_messages
                SET processed_on_utc = @ProcessedOnUtc,
                    error = @Error
                WHERE id = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString(),
            },
            transaction
        );
    }

    private async Task<IReadOnlyList<OutboxMessage>> GetOutboxMessages(
        IDbConnection connection,
        IDbTransaction transaction
    )
    {
        const string sql = """ 
            SELECT * FROM outbox_messages
            WHERE processed_on_utc is null
            ORDER BY occured_on_utc
            LIMIT @BatchSize
            FOR UPDATE 
            """;

        var result = await connection.QueryAsync<OutboxMessage>(
            sql,
            new { _outboxOptions.BatchSize },
            transaction
        );

        return result.ToList();
    }
}
