using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Infrastructure.Data.Seeding;

namespace RestaurantBooking.Infrastructure.Tests.Data.Seeding;

public class DatabaseSeederTests : IDisposable
{
    private readonly RestaurantBookingDbContext _context;
    private readonly DatabaseSeeder _seeder;

    public DatabaseSeederTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantBookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RestaurantBookingDbContext(options);
        _seeder = new DatabaseSeeder(_context);
    }

    [Fact]
    public async Task SeedAsync_Should_Create_Sample_Data()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var restaurantOwners = await _context.RestaurantOwners.ToListAsync();
        var restaurants = await _context.Restaurants.ToListAsync();
        var tables = await _context.Tables.ToListAsync();
        var customers = await _context.Customers.ToListAsync();

        restaurantOwners.Should().NotBeEmpty();
        restaurants.Should().NotBeEmpty();
        tables.Should().NotBeEmpty();
        customers.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SeedAsync_Should_Create_Restaurants_With_Tables()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var restaurants = await _context.Restaurants.ToListAsync();
        var tables = await _context.Tables.ToListAsync();

        restaurants.Should().HaveCountGreaterThan(0);
        tables.Should().HaveCountGreaterThan(0);

        // Verify that tables are associated with restaurants
        foreach (var table in tables)
        {
            restaurants.Should().Contain(r => r.Id == table.RestaurantId);
        }
    }

    [Fact]
    public async Task SeedAsync_Should_Not_Duplicate_Data_When_Called_Multiple_Times()
    {
        // Act
        await _seeder.SeedAsync();
        var firstRunCount = await _context.RestaurantOwners.CountAsync();

        await _seeder.SeedAsync();
        var secondRunCount = await _context.RestaurantOwners.CountAsync();

        // Assert
        secondRunCount.Should().Be(firstRunCount);
    }

    [Fact]
    public async Task SeedAsync_Should_Create_Operating_Hours_For_Restaurants()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var restaurants = await _context.Restaurants.ToListAsync();
        var operatingHours = await _context.OperatingHours.ToListAsync();

        restaurants.Should().NotBeEmpty();
        operatingHours.Should().NotBeEmpty();

        // Verify that operating hours are associated with restaurants
        foreach (var restaurant in restaurants)
        {
            operatingHours.Should().Contain(oh => oh.RestaurantId == restaurant.Id);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
