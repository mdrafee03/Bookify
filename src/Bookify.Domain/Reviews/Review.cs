using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews.Events;

namespace Bookify.Domain.Reviews;

public sealed class Review : Entity
{
    private Review(
        Guid id,
        Guid apartmentId,
        Guid userId,
        Guid bookingId,
        Rating rating,
        Comment comment,
        DateTime createdOnUtc
    )
        : base(id)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        BookingId = bookingId;
        Rating = rating;
        Comment = comment;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid ApartmentId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid BookingId { get; private set; }
    public Rating Rating { get; private set; }
    public Comment Comment { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static Result<Review> CreateReview(
        Guid apartmentId,
        Guid userId,
        Booking booking,
        Rating rating,
        Comment comment,
        DateTime utcNow
    )
    {
        if (booking.Status != BookingStatus.Completed)
        {
            return Result.Failure<Review>(ReviewErrors.BookingNotCompleted(booking.Id));
        }
        var review = new Review(
            Guid.CreateVersion7(),
            apartmentId,
            userId,
            booking.Id,
            rating,
            comment,
            utcNow
        );

        review.RaiseDomainEvent(new ReviewCreatedDomainEvent(review.Id));

        return review;
    }
}
