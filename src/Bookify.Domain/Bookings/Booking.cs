using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public class Booking : Entity
{
    private Booking(
        Guid id,
        Guid userId,
        Guid apartmentId,
        DateRange duration,
        Money priceForPeriod,
        Money cleaningFee,
        Money amenitiesUpCharge,
        Money totalPrice,
        BookingStatus status,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        ApartmentId = apartmentId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        CleaningFee = cleaningFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    private Booking()
        : this(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            null!,
            null!,
            null!,
            null!,
            null!,
            BookingStatus.Reserved,
            DateTime.MinValue
        ) { }

    public Guid UserId { get; private set; }
    public Guid ApartmentId { get; private set; }

    public DateRange Duration { get; private set; }
    public Money PriceForPeriod { get; private set; }
    public Money CleaningFee { get; private set; }
    public Money AmenitiesUpCharge { get; private set; }
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ConfirmedOnUtc { get; private set; }
    public DateTime? RejectedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public DateTime? CanceledOnUtc { get; private set; }

    public static Result<Booking> Reserve(
        Apartment apartment,
        Guid userId,
        DateRange duration,
        DateTime utcNow
    )
    {
        var pricingDetails = PricingService.CalculatePricing(apartment, duration);

        var booking = new Booking(
            Guid.CreateVersion7(),
            userId,
            apartment.Id,
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.CleaningFee,
            pricingDetails.AmenitiesUpcharge,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved,
            utcNow
        );

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        apartment.LastBookedOnUtc = utcNow;

        return booking;
    }

    public Result Confirm(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
        {
            return Result.Failure(BookingErrors.NotReserved);
        }
        Status = BookingStatus.Confirmed;
        ConfirmedOnUtc = utcNow;

        RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));
        return Result.Success();
    }

    public Result Reject(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
        {
            return Result.Failure(BookingErrors.NotReserved);
        }
        Status = BookingStatus.Rejected;
        RejectedOnUtc = utcNow;

        RaiseDomainEvent(new BookingRejectedDomainEvent(Id));
        return Result.Success();
    }

    public Result Complete(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
        {
            return Result.Failure(BookingErrors.NotConfirmed);
        }
        Status = BookingStatus.Completed;
        CompletedOnUtc = utcNow;

        RaiseDomainEvent(new BookingCompletedDomainEvent(Id));
        return Result.Success();
    }

    public Result Cancel(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
        {
            return Result.Failure(BookingErrors.NotConfirmed);
        }

        var currentDate = DateOnly.FromDateTime(utcNow);

        // if (currentDate > Duration.Start)
        // {
        //     return Result.Failure(BookingErrors.AlreadyStarted);
        // }

        Status = BookingStatus.Canceled;
        CanceledOnUtc = utcNow;

        RaiseDomainEvent(new BookingCanceledDomainEvent(Id));
        return Result.Success();
    }
}
