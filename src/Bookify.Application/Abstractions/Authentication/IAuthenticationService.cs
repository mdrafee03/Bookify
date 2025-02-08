using Bookify.Domain.Users;

namespace Bookify.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    public Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default
    );
}
