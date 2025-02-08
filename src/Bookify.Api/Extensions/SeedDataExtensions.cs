using System.Data;
using Bogus;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Apartments;
using Dapper;

namespace Bookify.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var sqlConnectionFactory =
            scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.CreateConnection();

        var faker = new Faker();

        AddApartmentSeed(connection, faker);
        AddUserSeed(connection, faker);
    }

    private static void AddApartmentSeed(IDbConnection connection, Faker faker)
    {
        List<object> apartments = [];
        for (var i = 0; i < 100; i++)
        {
            apartments.Add(
                new
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Company.CompanyName(),
                    Description = "Amazing view",
                    Country = faker.Address.Country(),
                    State = faker.Address.State(),
                    ZipCode = faker.Address.ZipCode(),
                    City = faker.Address.City(),
                    Street = faker.Address.StreetAddress(),
                    PriceAmount = faker.Random.Decimal(50, 1000),
                    PriceCurrency = "USD",
                    CleaningFeeAmount = faker.Random.Decimal(25, 200),
                    CleaningFeeCurrency = "USD",
                    Amenities = new List<int> { (int)Amenity.Parking, (int)Amenity.MountainView },
                    LastBookedOn = DateTime.MinValue,
                }
            );
        }

        const string sql = """
            INSERT INTO apartments
            (id, "name", description, address_country, address_state, address_zip_code, address_city, address_street, price_amount, price_currency, cleaning_fee_amount, cleaning_fee_currency, amenities, last_booked_on_utc)
            VALUES(@Id, @Name, @Description, @Country, @State, @ZipCode, @City, @Street, @PriceAmount, @PriceCurrency, @CleaningFeeAmount, @CleaningFeeCurrency, @Amenities, @LastBookedOn);
            """;

        connection.Execute(sql, apartments);
    }

    private static void AddUserSeed(IDbConnection connection, Faker faker)
    {
        List<object> users = [];
        for (var i = 0; i < 10; i++)
        {
            users.Add(
                new
                {
                    Id = Guid.NewGuid(),
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Email = faker.Internet.Email(),
                }
            );
        }

        const string sql = """
            INSERT INTO users
            (id, first_name, last_name, email)
            VALUES(@Id, @FirstName, @LastName, @Email);
            """;

        connection.Execute(sql, users);
    }
}
