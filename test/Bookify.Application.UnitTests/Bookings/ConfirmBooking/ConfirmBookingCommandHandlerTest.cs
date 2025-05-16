using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Bookify.Application.UnitTests.Bookings.ConfirmBooking;

public class ConfirmBookingCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUserContext _userContextMock;

    private readonly ConfirmBookingCommandHandler _handler;

    public ConfirmBookingCommandHandlerTest()
    {
        var loggerMock = Substitute.For<ILogger<ConfirmBookingCommandHandler>>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new ConfirmBookingCommandHandler(
            loggerMock,
            _unitOfWorkMock,
            dateTimeProviderMock,
            _bookingRepositoryMock,
            _userContextMock
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnsFailure_WhenBookingNotFound()
    {
        var command = new ConfirmBookingCommand(Guid.NewGuid());
        // Arrange
        _bookingRepositoryMock
            .GetByIdAsync(command.BookingId, Arg.Any<CancellationToken>())
            .Returns((Booking?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(BookingErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_Should_ReturnsFailure_WhenUserNotMatched()
    {
        // Assert
        var booking = BookingData.Create();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(Guid.CreateVersion7());

        // Act
        var result = await _handler.Handle(
            new ConfirmBookingCommand(booking.Id),
            CancellationToken.None
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_WhenBookingIsConfirmed()
    {
        // Assert
        var booking = BookingData.Create();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(booking.UserId);

        // Act
        await _handler.Handle(new ConfirmBookingCommand(booking.Id), CancellationToken.None);

        // Assert
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsConfirmed()
    {
        // Assert
        var booking = BookingData.Create();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(booking.UserId);

        // Act
        var result = await _handler.Handle(
            new ConfirmBookingCommand(booking.Id),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
    }
}
