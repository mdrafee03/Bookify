using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Users.ChangeUserRole;

public record ChangeUserRoleCommand(Guid UserId, string Role) : ICommand;
