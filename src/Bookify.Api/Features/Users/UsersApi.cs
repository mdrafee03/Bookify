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

        users.MapGet("/{id}", (string id, ISender sender) => { });
        users.MapPost("/register", RegisterUser);
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
}
