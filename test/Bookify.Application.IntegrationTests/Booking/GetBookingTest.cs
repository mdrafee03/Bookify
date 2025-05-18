using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Xunit;

namespace Bookify.Application.IntegrationTests.Booking;

public class GetBookingTest : BaseIntegrationTest
{
    private readonly IntegrationTestWebFactory _factory;

    public GetBookingTest(IntegrationTestWebFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    private async Task SeedDTestData()
    {
        var apartment = new Apartment(
            Guid.CreateVersion7(),
            new Name("Name"),
            new Description("description"),
            new Address("country", "state", "city", "zipcode", "street"),
            new Money(100, Currency.Usd),
            new Money(100, Currency.Usd),
            DateTime.UtcNow
        );

        AppDbContext.Apartments.Add(apartment);

        var user = User.CreateUser(
            new FirstName("firstName"),
            new LastName("lastName"),
            new Email("email")
        );

        AppDbContext.Attach(user.UserRoles.First());

        AppDbContext.Users.Add(user);

        var dateRange = DateRange.Create(
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        var booking = Domain.Bookings.Booking.Reserve(
            apartment,
            user.Id,
            dateRange,
            DateTime.UtcNow
        );
        AppDbContext.Bookings.Add(booking.Value);
        await UnitOfWork.SaveChangesAsync();
    }

    [Fact]
    public async Task GetBooking_Should_ReturnFailure_WhenBookingNotFound()
    {
        // Act
        var result = await Sender.Send(new GetBookingQuery(Guid.CreateVersion7()));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotFound);
    }

    [Fact]
    public async Task GetBooking_Should_ReturnFailure_WhenUserNotMatched()
    {
        // Arrange
        await SeedDTestData();

        // Act
        var result = await Sender.Send(new GetBookingQuery(AppDbContext.Bookings.First().Id));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotFound);
    }

    [Fact(Skip = "Not implemented yet")]
    public async Task GetBooking_Should_ReturnSuccess_WhenBookingFound()
    {
        // Arrange
        await SeedDTestData();
        var bookingId = AppDbContext.Bookings.First().Id;
        var userId = AppDbContext.Bookings.First().UserId;

        var userContextMock = Substitute.For<IUserContext>();
        userContextMock.UserId.Returns(userId);
        userContextMock.IdentityId.Returns("identityId");

        // Act
        var result = await Sender.Send(new GetBookingQuery(bookingId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(bookingId);
    }
}
