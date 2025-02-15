using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(AppDbContext dbContext)
{
    public async Task<UserRolesResponse?> GetRolesForUser(string identityId)
    {
        var userRoles = await dbContext
            .Users.Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse()
            {
                UserId = user.Id,
                Roles = user.UserRoles.ToList(),
            })
            .FirstOrDefaultAsync();

        return userRoles;
    }
}
