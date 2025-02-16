using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    public Task ChangeRoleAsync(Guid userId, string role, CancellationToken cancellationToken);

    public Task AssignPermissionAsync(
        Guid userId,
        IEnumerable<string> permissions,
        CancellationToken cancellationToken
    );
}
