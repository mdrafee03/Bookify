using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Users.AssignPermission;

public class AssignPermissionCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    : ICommandHandler<AssignPermissionCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Unit>> Handle(
        AssignPermissionCommand request,
        CancellationToken cancellationToken
    )
    {
        await _userRepository.AssignPermissionAsync(
            request.UserId,
            request.Permissions,
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
