using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Reviews;

public static class ReviewErrors
{
    public static Error BookingNotCompleted(Guid bookingId) =>
        new Error("Booking.NotCompleted", $"Booking with id {bookingId} has not been completed.");
}
