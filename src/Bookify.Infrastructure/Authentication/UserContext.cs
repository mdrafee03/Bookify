using Bookify.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bookify.Infrastructure.Authentication;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserId =>
        _httpContextAccessor.HttpContext?.User.GetUserId()
        ?? throw new InvalidOperationException("User id is missing");

    public string IdentityId =>
        _httpContextAccessor.HttpContext?.User.GetIdentityId()
        ?? throw new InvalidOperationException("Identity id is missing");
}
