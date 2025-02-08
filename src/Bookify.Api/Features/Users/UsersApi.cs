using Carter;
using MediatR;

namespace Bookify.Api.Features.Users;

public class UsersApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("users");

        users.MapGet("/{id}", (string id, ISender sender) => { });
    }
}
