using Bookify.Application.Abstractions.Messaging;
using MediatR;

namespace Bookify.Application.Users.AssignPermission;

public sealed record AssignPermissionCommand(Guid UserId, IEnumerable<string> Permissions)
    : ICommand<Unit>;
