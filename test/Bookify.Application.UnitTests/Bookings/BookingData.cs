using Bookify.Application.UnitTests.Apartments;
using Bookify.Domain.Bookings;

namespace Bookify.Application.UnitTests.Bookings;

public static class BookingData
{
    public static Booking Create()
    {
        var dateRange = DateRange.Create(
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );
        var booking = Booking.Reserve(
            ApartmentData.Create(),
            Guid.CreateVersion7(),
            dateRange,
            DateTime.Now
        );

        return booking.Value;
    }

    public static Booking CreateConfirmedBooking()
    {
        var booking = BookingData.Create();

        booking.Confirm(DateTime.UtcNow);

        return booking;
    }
}
