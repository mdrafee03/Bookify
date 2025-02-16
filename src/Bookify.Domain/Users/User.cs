using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

public sealed class User : Entity
{
    private readonly List<Role> _userRoles = [];
    private readonly List<Permission> _permissions = [];

    private User(Guid id, FirstName firstName, LastName lastName, Email email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Email Email { get; private set; }

    public string IdentityId { get; private set; } = string.Empty;

    public IReadOnlyCollection<Role> UserRoles => _userRoles.AsReadOnly();

    public IReadOnlyCollection<Permission> Permissions => _permissions;

    public static User CreateUser(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.CreateVersion7(), firstName, lastName, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        user._userRoles.Add(Role.Registered);

        return user;
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }

    public void ChangeRole(Role role)
    {
        _userRoles.Clear();
        _userRoles.Add(role);
    }

    public void AssignPermissions(List<Permission> permissions)
    {
        _permissions.AddRange(permissions);
    }
}
