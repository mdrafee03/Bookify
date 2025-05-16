using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Application.Exceptions;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Bookify.Application.UnitTests.Bookings.ReserveBooking;

public class ReserveBookingCommandHandlerTest
{
    private static readonly ReserveBookingCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        DateOnly.FromDateTime(DateTime.UtcNow),
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
    );

    private static readonly User UserData = User.CreateUser(
        new FirstName("John"),
        new LastName("Doe"),
        new Email("email")
    );

    private readonly IApartmentRepository _apartmentRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    private readonly ReserveBookingCommandHandler _handler;

    public ReserveBookingCommandHandlerTest()
    {
        _apartmentRepositoryMock = Substitute.For<IApartmentRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

        dateTimeProviderMock.UtcNow.Returns(DateTime.UtcNow);

        _handler = new ReserveBookingCommandHandler(
            _apartmentRepositoryMock,
            _userRepositoryMock,
            _bookingRepositoryMock,
            _unitOfWorkMock,
            dateTimeProviderMock
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserIsNull()
    {
        // Arrange
        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)(null));

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        result.Error.Should().Be(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenApartmentIsNull()
    {
        // Arrange
        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns(UserData);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns((Apartment?)null);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        result.Error.Should().Be(ApartmentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenBookingIsOverlapping()
    {
        // Arrange
        var apartment = ApartmentData.Create();

        var dateRange = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns(UserData);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, dateRange, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        result.Error.Should().Be(BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenConcurrencyExceptionIsThrown()
    {
        // Arrange
        var apartment = ApartmentData.Create();

        var dateRange = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns(UserData);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, dateRange, Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConcurrencyException("Concurrency error", new Exception()));

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        result.Error.Should().Be(BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsReserved()
    {
        // Arrange
        var apartment = ApartmentData.Create();

        var dateRange = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns(UserData);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, dateRange, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_CallRepositoryAdd_WhenBookingIsReserved()
    {
        // Arrange
        var apartment = ApartmentData.Create();

        var dateRange = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns(UserData);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, dateRange, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        _bookingRepositoryMock.Received(1).Add(Arg.Is<Booking>(b => b.Id == result.Value));
    }

    [Fact]
    public async Task Handle_Should_CallUnitOfWorkSaveChangesAsync()
    {
        // Arrange
        var apartment = ApartmentData.Create();

        var dateRange = DateRange.Create(Command.StartDate, Command.EndDate);

        _userRepositoryMock
            .GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
            .Returns(UserData);

        _apartmentRepositoryMock
            .GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        _bookingRepositoryMock
            .IsOverlappingAsync(apartment, dateRange, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(Command, CancellationToken.None);

        // Assert
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
