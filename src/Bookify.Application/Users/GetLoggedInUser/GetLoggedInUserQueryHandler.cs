using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Users.GetLoggedInUser;

internal sealed class GetLoggedInUserQueryHandler(
    ISqlConnectionFactory connectionFactory,
    IUserContext userContext,
    ILogger<GetLoggedInUserQueryHandler> logger
) : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactoryFactory = connectionFactory;
    private readonly IUserContext _userContext = userContext;
    private readonly ILogger<GetLoggedInUserQueryHandler> _logger = logger;

    public async Task<Result<UserResponse>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Getting logged in user for {@Request} and userContext {@Context}",
            request,
            _userContext
        );

        var connection = _sqlConnectionFactoryFactory.CreateConnection();

        const string sql = """
            SELECT
                u.id AS Id,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.email AS Email
            FROM users AS u
            WHERE u.identity_id = @IdentityId
            """;

        var command = new CommandDefinition(
            sql,
            new { _userContext.IdentityId },
            cancellationToken: cancellationToken
        );

        var user = await connection.QueryFirstOrDefaultAsync<UserResponse>(command);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);
        }

        return user;
    }
}
