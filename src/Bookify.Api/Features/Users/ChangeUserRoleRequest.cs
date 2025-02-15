namespace Bookify.Api.Features.Users;

public sealed record ChangeUserRoleRequest(Guid UserId, string Role);
