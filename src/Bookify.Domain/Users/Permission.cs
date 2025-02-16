namespace Bookify.Domain.Users;

public sealed class Permission(int id, string name)
{
    public static readonly Permission UsersRead = new Permission(1, "Users:Read");
    public int Id { get; init; } = id;

    public string Name { get; init; } = name;

    public ICollection<User> Users { get; init; } = [];
}
