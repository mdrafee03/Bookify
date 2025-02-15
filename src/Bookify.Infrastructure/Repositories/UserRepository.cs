using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository(AppDbContext dbContext)
    : Repository<User>(dbContext),
        IUserRepository
{
    public override void Add(User user)
    {
        foreach (var userRole in user.UserRoles)
        {
            DbContext.Attach(userRole);
        }

        base.Add(user);
    }

    public async Task ChangeRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
    {
        var user = await DbContext
            .Users.Include(user => user.UserRoles)
            .FirstOrDefaultAsync(entity => entity.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new EntityNotFoundException<User>();
        }

        var roleEntity = await DbContext.Roles.FirstOrDefaultAsync(
            entity => entity.Name == RoleName.FromString(role),
            cancellationToken
        );

        if (roleEntity is null)
        {
            throw new EntityNotFoundException<Role>();
        }

        user.ChangeRole(roleEntity);

        DbContext.Update(user);
    }
}
