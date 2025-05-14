using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using FluentAssertions;
using Xunit;

namespace Bookify.Domain.UnitTests.Bookings;

public class PricingServiceTest
{
    [Fact]
    public void CalculatePrice_ShouldReturnCorrectPrice_WhenValidInput()
    {
        // Arrange
        var startDate = new DateOnly(2023, 10, 1);
        var endDate = new DateOnly(2023, 10, 5);
        var price = new Money(100, Currency.Usd);

        var apartment = ApartmentData.Create(price);

        var period = DateRange.Create(startDate, endDate);

        var expectedPrice = apartment.Price with
        {
            Amount = apartment.Price.Amount * period.LengthInDays,
        };

        // Act
        var pricingDetails = PricingService.CalculatePricing(apartment, period);

        // Assert
        pricingDetails.TotalPrice.Should().Be(expectedPrice);
    }

    [Fact]
    public void CalculatePrice_ShouldReturnCorrectPrice_WhenCleaningFeeIncluded()
    {
        var startDate = new DateOnly(2023, 10, 1);
        var endDate = new DateOnly(2023, 10, 5);
        var price = new Money(100, Currency.Usd);
        var cleaningFee = new Money(100, Currency.Usd);

        var apartment = ApartmentData.Create(price, cleaningFee);

        var period = DateRange.Create(startDate, endDate);

        var expectedPrice = apartment.Price with
        {
            Amount = apartment.Price.Amount * period.LengthInDays + cleaningFee.Amount,
        };

        // Act
        var pricingDetails = PricingService.CalculatePricing(apartment, period);

        // Assert
        pricingDetails.TotalPrice.Should().Be(expectedPrice);
    }
}
