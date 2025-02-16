using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(AppDbContext dbContext)
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<UserRolesResponse?> GetRolesForUser(string identityId)
    {
        var userRoles = await _dbContext
            .Users.Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse()
            {
                UserId = user.Id,
                Roles = user.UserRoles.ToList(),
            })
            .FirstOrDefaultAsync();

        return userRoles;
    }

    public async Task<HashSet<string>> GetPermissionsForUser(string identityId)
    {
        var userPermissions = await _dbContext
            .Users.Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Permissions)
            .ToListAsync();

        return userPermissions.Select(permission => permission.Name).ToHashSet();
    }
}
