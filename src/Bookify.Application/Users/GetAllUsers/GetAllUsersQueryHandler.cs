using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Domain.Abstractions;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Users.GetAllUsers;

internal sealed class GetAllUsersQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ILogger<GetAllUsersQueryHandler> logger
) : IQueryHandler<GetAllUsersQuery, List<UserResponse>>
{
    private readonly ILogger<GetAllUsersQueryHandler> _logger = logger;
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result<List<UserResponse>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        const string sql = """
            SELECT
                u.id AS Id,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.email AS Email
            FROM users AS u
            """;

        var connection = _sqlConnectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<UserResponse>(command);
        var users = result.ToList();

        return users;
    }
}
