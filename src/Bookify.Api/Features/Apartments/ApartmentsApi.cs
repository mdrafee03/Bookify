using Bookify.Application.Apartments.SearchApartments;
using Bookify.Domain.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Features.Apartments;

public sealed class ApartmentsApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var apartments = app.MapGroup("apartments");

        apartments.MapGet("/searchApartments", SearchApartment);
    }

    private static async Task<
        Results<Ok<IReadOnlyList<ApartmentResponse>>, NotFound<Error>>
    > SearchApartment(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new SearchApartmentsQuery(startDate, endDate);
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}
