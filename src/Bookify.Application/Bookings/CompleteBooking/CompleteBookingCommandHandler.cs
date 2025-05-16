using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Bookings.CompleteBooking;

internal sealed class CompleteBookingCommandHandler(
    ILogger<CompleteBookingCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    IBookingRepository bookingRepository,
    IUserContext userContext
) : ICommandHandler<CompleteBookingCommand>
{
    private readonly ILogger<CompleteBookingCommandHandler> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result> Handle(
        CompleteBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Completing booking with id {Id}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking is null)
        {
            _logger.LogWarning("Booking with id {Id} not found", request.BookingId);
            return Result.Failure(BookingErrors.NotFound);
        }

        if (booking.UserId != _userContext.UserId)
        {
            _logger.LogWarning(
                "User {UserId} is not authorized to complete booking with id {Id}",
                _userContext.UserId,
                request.BookingId
            );
            return Result.Failure(BookingErrors.NotAuthorized);
        }

        var result = booking.Complete(_dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking with id {Id} completed", request.BookingId);
        return Result.Success();
    }
}
