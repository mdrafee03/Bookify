using Bookify.Domain.Abstractions;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Apartments;

public sealed class Apartment(
    Guid id,
    Name name,
    Description description,
    Address address,
    Money price,
    Money cleaningFee,
    DateTime? lastBookedOnUtc
) : Entity(id)
{
    private Apartment()
        : this(Guid.Empty, null!, null!, null!, null!, null!, null) { }

    public Name Name { get; private set; } = name;

    public Description Description { get; private set; } = description;

    public Address Address { get; private set; } = address;

    public Money Price { get; private set; } = price;

    public Money CleaningFee { get; private set; } = cleaningFee;

    public DateTime? LastBookedOnUtc { get; internal set; } = lastBookedOnUtc;

    public List<Amenity> Amenities { get; private set; } = [];
}
