using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Bookings.CancelBooking;

internal sealed class CancelBookingCommandHandler(
    ILogger<CancelBookingCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    IBookingRepository bookingRepository,
    IUserContext userContext
) : ICommandHandler<CancelBooking.CancelBookingCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<Result> Handle(
        CancelBooking.CancelBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Canceling booking for {BookingId}", request.BookingId);

        var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking is null)
        {
            return Result.Failure<bool>(BookingErrors.NotFound);
        }

        if (booking.UserId != _userContext.UserId)
        {
            return Result.Failure<bool>(BookingErrors.NotAuthorized);
        }

        var result = booking.Cancel(_dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
