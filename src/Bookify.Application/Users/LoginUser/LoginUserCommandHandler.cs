using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler(IJwtService jwtService)
    : ICommandHander<LoginUserCommand, AccessTokenResponse>
{
    private readonly IJwtService _jwtService = jwtService;

    public async Task<Result<AccessTokenResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _jwtService.GetAccessTokenAsync(
            request.Email,
            request.Password,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
        }

        return new AccessTokenResponse(result.Value);
    }
}
