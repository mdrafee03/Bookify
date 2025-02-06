using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public class PricingService
{
    public static PricingDetails CalculatePricing(Apartment apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;
        var price = apartment.Price.Amount;

        var priceForPeriod = new Money(price * period.LengthInDays, currency);

        var percentageUpCharge = apartment.Amenities.Sum(amenity =>
            amenity switch
            {
                Amenity.GardenView or Amenity.MountainView => 0.05m,
                Amenity.AirConditioning => 0.01m,
                Amenity.Parking => 0.01m,
                _ => 0,
            }
        );

        var amenitiesUpCharge =
            percentageUpCharge > 0
                ? new Money(priceForPeriod.Amount * percentageUpCharge, currency)
                : Money.Zero(currency);

        var totalPrice = priceForPeriod + amenitiesUpCharge + apartment.CleaningFee;

        return new PricingDetails(
            priceForPeriod,
            apartment.CleaningFee,
            amenitiesUpCharge,
            totalPrice
        );
    }
}
