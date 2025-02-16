namespace Bookify.Api.Features.Users;

public sealed record AssignPermissionsRequest(Guid UserId, IEnumerable<string> Permissions);
