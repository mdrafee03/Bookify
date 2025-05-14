using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookify.Domain.UnitTests.Bookings;

public class BookingTest : BaseTest
{
    [Fact]
    public void Reserve_Should_RaiseBookingReservedDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var startDate = new DateOnly(2023, 10, 1);
        var endDate = new DateOnly(2023, 10, 7);

        var price = new Money(100, Currency.Usd);
        var cleaningFee = new Money(100, Currency.Usd);

        var apartment = ApartmentData.Create(price, cleaningFee);

        var duration = DateRange.Create(startDate, endDate);

        // Act
        var booking = Booking.Reserve(apartment, userId, duration, DateTime.UtcNow);

        // Assert
        var domainEvents = AssertDomainEventWasPublished<BookingReservedDomainEvent>(booking.Value);

        domainEvents.Should().NotBeNull();
        domainEvents.BookingId.Should().Be(booking.Value.Id);
    }
}
