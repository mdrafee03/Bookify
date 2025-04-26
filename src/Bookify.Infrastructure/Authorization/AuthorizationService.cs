using Bookify.Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(AppDbContext dbContext, ICacheService cache)
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ICacheService _cache = cache;

    public async Task<UserRolesResponse?> GetRolesForUser(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";
        var cachedRoles = await _cache.GetAsync<UserRolesResponse>(cacheKey);

        if (cachedRoles is not null)
        {
            return cachedRoles;
        }

        var userRoles = await _dbContext
            .Users.Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse()
            {
                UserId = user.Id,
                Roles = user.UserRoles.ToList(),
            })
            .FirstOrDefaultAsync();

        if (userRoles is not null)
        {
            await _cache.SetAsync(cacheKey, userRoles);
        }
        return userRoles;
    }

    public async Task<HashSet<string>> GetPermissionsForUser(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";
        var cachedPermissions = await _cache.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null)
        {
            return cachedPermissions;
        }
        var userPermissions = await _dbContext
            .Users.Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Permissions)
            .ToListAsync();

        var permissionsSet = userPermissions.Select(permission => permission.Name).ToHashSet();
        await _cache.SetAsync(cacheKey, permissionsSet);

        return permissionsSet;
    }
}
