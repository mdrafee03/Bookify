using Bookify.Application.Apartments.SearchApartments;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Features.Apartments;

public sealed class ApartmentsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var apartments = app.MapGroup("apartments");

        apartments.MapGet(
            "/searchApartments",
            async ([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate, ISender sender) =>
            {
                var query = new SearchApartmentsQuery(startDate, endDate);
                var result = await sender.Send(query);
                return TypedResults.Ok(result);
            }
        );
    }
}
