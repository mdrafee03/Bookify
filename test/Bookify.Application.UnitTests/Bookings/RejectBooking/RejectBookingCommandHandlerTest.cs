using System;
using System.Threading;
using System.Threading.Tasks;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.RejectBooking;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Bookify.Application.UnitTests.Bookings.RejectBooking;

public class RejectBookingCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUserContext _userContextMock;

    private readonly RejectBookingCommandHandler _handler;

    public RejectBookingCommandHandlerTest()
    {
        var loggerMock = Substitute.For<ILogger<RejectBookingCommandHandler>>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new RejectBookingCommandHandler(
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
        var command = new RejectBookingCommand(Guid.NewGuid());
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
            new RejectBookingCommand(booking.Id),
            CancellationToken.None
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_WhenBookingIsRejected()
    {
        // Assert
        var booking = BookingData.Create();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(booking.UserId);

        // Act
        await _handler.Handle(new RejectBookingCommand(booking.Id), CancellationToken.None);

        // Assert
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsRejected()
    {
        // Assert
        var booking = BookingData.Create();

        _bookingRepositoryMock
            .GetByIdAsync(booking.Id, Arg.Any<CancellationToken>())
            .Returns(booking);

        _userContextMock.UserId.Returns(booking.UserId);

        // Act
        var result = await _handler.Handle(
            new RejectBookingCommand(booking.Id),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
    }
}
