using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookify.Application.IntegrationTests.Apartments;

public class SearchApartmentsTest : BaseIntegrationTest
{
    public SearchApartmentsTest(IntegrationTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task SearchApartments_Should_ReturnEmptyList_WhenDateRangeIsInvalid()
    {
        // Arrange
        var query = new SearchApartmentsQuery(
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchApartments_Should_ReturnList_WhenDateRangeIsValid()
    {
        // Arrange
        var query = new SearchApartmentsQuery(
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
