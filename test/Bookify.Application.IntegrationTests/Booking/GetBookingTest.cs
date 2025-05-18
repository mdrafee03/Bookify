using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Bookings;
using FluentAssertions;
using Xunit;

namespace Bookify.Application.IntegrationTests.Booking;

public class GetBookingTest : BaseIntegrationTest
{
    public GetBookingTest(IntegrationTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetBooking_Should_ReturnFailure_WhenBookingNotFound()
    {
        // Arrange
        var bookingId = Guid.CreateVersion7();

        // Act
        var result = await Sender.Send(new GetBookingQuery(bookingId));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotFound);
    }
}
