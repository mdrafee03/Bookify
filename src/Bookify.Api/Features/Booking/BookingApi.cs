using Bookify.Application.Bookings.CancelBookingCommand;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Features.Booking;

public sealed class BookingApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var bookings = app.MapGroup("/bookings");

        bookings.MapGet("/{bookingId:guid}", GetBooking).MapToApiVersion(1);
        bookings.MapGet("/{bookingId:guid}", GetBookingV2).MapToApiVersion(2);
        bookings.MapPost("/reserve-booking", ReserveBooking);
        bookings.MapPost("/cancel-booking", CancelBooking);
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

    private static async Task<Results<Ok<BookingResponse>, NotFound<Error>>> GetBookingV2(
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

    private static async Task<IResult> CancelBooking(
        [FromBody] CancelBookingRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new CancelBookingCommand(request.BookingId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok();
    }
}
