using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bookify.Infrastructure.Authentication;

public static class ClaimPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal?.FindFirstValue(ClaimTypes.UserData);
        if (userId == null)
        {
            throw new InvalidOperationException("User id claim is missing");
        }

        return Guid.TryParse(userId, out var parsedUserId)
            ? parsedUserId
            : throw new InvalidOperationException("User id is not parsable");
    }

    public static string GetIdentityId(this ClaimsPrincipal principal)
    {
        var identityId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (identityId == null)
        {
            throw new InvalidOperationException("Identity id claim is missing");
        }

        return identityId;
    }
}
