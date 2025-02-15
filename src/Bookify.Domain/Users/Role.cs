namespace Bookify.Domain.Users;

public sealed class Role
{
    public static readonly Role Registered = new Role
    {
        Id = 1,
        Name = new RoleName(RoleType.Registered),
    };
    public static readonly Role Admin = new Role { Id = 2, Name = new RoleName(RoleType.Admin) };
    public int Id { get; init; }

    public required RoleName Name { get; init; }

    public ICollection<User> Users { get; init; } = [];
}
