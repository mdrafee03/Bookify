using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationPolicyProvider(
    IOptions<AuthorizationOptions> authorizationOptions
) : DefaultAuthorizationPolicyProvider(authorizationOptions)
{
    private readonly AuthorizationOptions _authorizationOptions = authorizationOptions.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _authorizationOptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}
