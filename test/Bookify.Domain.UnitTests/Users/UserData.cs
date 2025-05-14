using Bookify.Domain.Users;

namespace Bookify.Domain.UnitTests.Users;

public static class UserData
{
    public static readonly FirstName FirstName = new("FirstName");
    public static readonly LastName LastName = new("LastName");
    public static readonly Email Email = new("test@test.com");
}
