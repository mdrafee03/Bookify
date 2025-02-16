using Bookify.Application.Users.AssignPermission;
using Bookify.Application.Users.ChangeUserRole;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LoginUser;
using Bookify.Application.Users.RegisterUser;
using Bookify.Domain.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bookify.Api.Features.Users;

public sealed class UsersApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("users");

        users.MapGet("/me", GetLoggedInUser).RequireAuthorization(Permissions.UsersRead);
        users.MapPost("/login", Login);
        users.MapPost("/register", RegisterUser);
        users
            .MapPut("/change-role", ChangeUserRole)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));
        users
            .MapPut("/assign-permissions", AssignUserPermissions)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));
    }

    private static async Task<Results<Ok<UserResponse>, BadRequest<Error>>> GetLoggedInUser(
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(new GetLoggedInUserQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<AccessTokenResponse>, BadRequest<Error>>> Login(
        LoginUserRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<Guid>, BadRequest<Error>>> RegisterUser(
        RegisterUserRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new RegisterUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> ChangeUserRole(
        ChangeUserRoleRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new ChangeUserRoleCommand(request.UserId, request.Role);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok();
    }

    private static async Task<IResult> AssignUserPermissions(
        AssignPermissionsRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new AssignPermissionCommand(request.UserId, request.Permissions);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.NoContent();
    }
}
