using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.ChangeUserRole;

internal sealed class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<ChangeUserRoleCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(
        ChangeUserRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        await _userRepository.ChangeRoleAsync(request.UserId, request.Role, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
