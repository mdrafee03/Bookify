using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.GetLoggedInUser;

namespace Bookify.Application.Users.GetAllUsers;

public sealed record GetAllUsersQuery : IQuery<List<UserResponse>>;
