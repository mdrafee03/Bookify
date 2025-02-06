using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository(AppDbContext dbContext)
    : Repository<User>(dbContext),
        IUserRepository { }
