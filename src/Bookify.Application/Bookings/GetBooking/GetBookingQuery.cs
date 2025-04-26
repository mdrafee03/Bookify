using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;

namespace Bookify.Application.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>, ICacheQuery
{
    public string CacheKey => $"GetBooking-{BookingId}";
    public TimeSpan? Expiration => null;
}