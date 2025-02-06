using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public static class UserErrors
{
    public static Error UserNotFound =>
        new("User.Found", "The user with specified id was not found.");
}
