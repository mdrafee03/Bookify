using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.UnitTests.Apartments;

public static class ApartmentData
{
    public static Apartment Create(Money price, Money? cleaningFee = null)
    {
        return new Apartment(
            Guid.NewGuid(),
            new Name("Apartment Name"),
            new Description("Apartment Description"),
            new Address("country", "state", "city", "zip", "street"),
            price,
            cleaningFee ?? Money.Zero(Currency.Usd),
            DateTime.UtcNow
        );
    }
}
