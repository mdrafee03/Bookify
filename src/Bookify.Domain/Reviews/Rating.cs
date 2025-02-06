using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Reviews;

public sealed record Rating
{
    public static readonly Error InvalidRatingError = new(
        "Rating.InvalidRating",
        "Rating must be between 1 and 5"
    );

    private Rating(int value) => Value = value;

    public int Value { get; }

    public static Result<Rating> Create(int value)
    {
        return value is < 1 or > 5 ? Result.Failure<Rating>(InvalidRatingError) : new Rating(value);
    }
}
