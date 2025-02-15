namespace Bookify.Domain.Users;

public sealed record RoleName(RoleType Value)
{
    public static RoleName FromString(string roleName)
    {
        if (!Enum.TryParse<RoleType>(roleName, ignoreCase: true, out var parsedRole))
        {
            throw new ArgumentException($"Invalid role name: {roleName}", nameof(roleName));
        }

        return new RoleName(parsedRole);
    }
};
