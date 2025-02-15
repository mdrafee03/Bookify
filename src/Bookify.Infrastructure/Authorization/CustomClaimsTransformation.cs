using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;

internal sealed class CustomClaimsTransformation(IServiceProvider serviceProvider)
    : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (
            principal.HasClaim(claim => claim.Type == ClaimTypes.Role)
            && principal.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier)
        )
        {
            return principal;
        }

        var scope = serviceProvider.CreateScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        var identityId = principal.GetIdentityId();

        var roles = await authorizationService.GetRolesForUser(identityId);

        if (roles is null)
        {
            throw new InvalidOperationException("User roles are missing");
        }

        var claimsIdentity = new ClaimsIdentity();

        var claims = new List<Claim> { new Claim(ClaimTypes.UserData, roles.UserId.ToString()) };

        claims.AddRange(
            roles.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name.Value.ToString()))
        );

        claimsIdentity.AddClaims(claims);

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}
