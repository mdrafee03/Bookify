using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Application.UnitTests.Apartments;

public static class ApartmentData
{
    public static Apartment Create()
    {
        return new Apartment(
            Guid.NewGuid(),
            new Name("Apartment Name"),
            new Description("Apartment Description"),
            new Address("country", "state", "city", "zip", "street"),
            new Money(100, Currency.Usd),
            Money.Zero(Currency.Usd),
            DateTime.UtcNow
        );
    }
}
