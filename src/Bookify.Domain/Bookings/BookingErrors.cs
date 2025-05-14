using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings;

public static class BookingErrors
{
    public static Error NotFound =>
        new("Booking.Found", "The booking with specified id was not found.");
    public static Error Overlap =>
        new("Booking.Overlap", "The booking overlaps with another booking.");
    public static Error NotReserved => new("Booking.NotReserved", "The booking is not reserved.");
    public static Error NotConfirmed =>
        new("Booking.NotConfirmed", "The booking is not confirmed.");
    public static Error AlreadyStarted =>
        new("Booking.AlreadyStarted", "The booking has already started.");

    public static Error NotAuthorized =>
        new("Booking.NotAuthorized", "The user is not authorized to perform this action.");
}
