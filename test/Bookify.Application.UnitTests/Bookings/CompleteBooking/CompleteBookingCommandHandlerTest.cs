using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.CompleteBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Bookify.Application.UnitTests.Bookings.CompleteBooking;

public class CompleteBookingCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUserContext _userContextMock;

    private readonly CompleteBookingCommandHandler _handler;

    public CompleteBookingCommandHandlerTest()
    {
        var loggerMock = Substitute.For<ILogger<CompleteBookingCommandHandler>>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new CompleteBookingCommandHandler(
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
        var command = new CompleteBookingCommand(Guid.NewGuid());
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
        var booking = BookingData.CreateConfirmedBooking();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(Guid.CreateVersion7());

        // Act
        var result = await _handler.Handle(
            new CompleteBookingCommand(booking.Id),
            CancellationToken.None
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_WhenBookingIsCompleted()
    {
        // Assert
        var booking = BookingData.CreateConfirmedBooking();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(booking.UserId);

        // Act
        await _handler.Handle(new CompleteBookingCommand(booking.Id), CancellationToken.None);

        // Assert
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsCompleted()
    {
        // Assert
        var booking = BookingData.CreateConfirmedBooking();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(booking.UserId);

        // Act
        var result = await _handler.Handle(
            new CompleteBookingCommand(booking.Id),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
    }
}
