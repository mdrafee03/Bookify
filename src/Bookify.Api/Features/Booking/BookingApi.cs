using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bookify.Api.Features.Booking;

public sealed class BookingApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var bookings = app.MapGroup("bookings");

        bookings
            .MapGet("/{bookingId:guid}", GetBooking)
            .RequireAuthorization(Permissions.UsersRead);

        bookings.MapPost("/reserveBooking", ReserveBooking);
    }

    private static async Task<Results<Ok<BookingResponse>, NotFound<Error>>> GetBooking(
        Guid bookingId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetBookingQuery(bookingId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound(result.Error);
        }
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<Guid>, BadRequest<Error>>> ReserveBooking(
        ReserveBookingRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new ReserveBookingCommand(
            request.ApartmentId,
            request.UserId,
            request.StartDate,
            request.EndDate
        );
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }
}
