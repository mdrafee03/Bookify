using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var name = request.GetType().Name;

            _logger.LogInformation("Handling command {CommandName} ({@Command})", name, request);

            var result = await next();

            _logger.LogInformation("Command {CommandName} processed successfully", name);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Command {CommandName} failed: {Message}",
                request.GetType().Name,
                ex.Message
            );
            throw;
        }
    }
}
