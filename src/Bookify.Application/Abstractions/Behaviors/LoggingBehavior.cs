using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Bookify.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var name = request.GetType().Name;

            _logger.LogInformation("Handling request {Request} ({@Command})", name, request);

            var result = await next();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Request {RequestName} processed successfully", name);
            }
            else
            {
                using (LogContext.PushProperty("Errors", result.Error, true))
                {
                    _logger.LogError("Request {Request} processed with error", name);
                }
            }

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
