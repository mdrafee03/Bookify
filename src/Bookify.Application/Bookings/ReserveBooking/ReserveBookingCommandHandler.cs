using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandHandler(
    IApartmentRepository apartmentRepository,
    IUserRepository userRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<ReserveBookingCommand, Guid>
{
    private readonly IApartmentRepository _apartmentRepository = apartmentRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<Result<Guid>> Handle(
        ReserveBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.UserNotFound);
        }

        var apartment = await _apartmentRepository.GetByIdAsync(
            request.ApartmentId,
            cancellationToken
        );

        if (apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound);
        }

        var duration = DateRange.Create(request.StartDate, request.EndDate);

        if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }

        try
        {
            var booking = Booking.Reserve(
                apartment,
                request.UserId,
                duration,
                _dateTimeProvider.UtcNow
            );

            _bookingRepository.Add(booking.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return booking.Value.Id;
        }
        catch (ConcurrencyException ex)
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
    }
}
