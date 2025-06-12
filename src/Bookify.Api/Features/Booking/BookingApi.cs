using Bookify.Application.Bookings.CancelBooking;
using Bookify.Application.Bookings.CompleteBooking;
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.RejectBooking;
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
        bookings.MapPost("/confirm-booking", ConfirmBooking);
        bookings.MapPost("/reject-booking", RejectBooking);
        bookings.MapPost("/cancel-booking", CancelBooking);
        bookings.MapPost("/complete-booking", CompleteBooking);
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

    private static async Task<IResult> ConfirmBooking(
        [FromBody] ConfirmBookingRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new ConfirmBookingCommand(request.BookingId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok();
    }

    private static async Task<IResult> RejectBooking(
        [FromBody] RejectBookingRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new RejectBookingCommand(request.BookingId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok();
    }

    private static async Task<IResult> CompleteBooking(
        [FromBody] CompleteBookingRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new CompleteBookingCommand(request.BookingId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok();
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
