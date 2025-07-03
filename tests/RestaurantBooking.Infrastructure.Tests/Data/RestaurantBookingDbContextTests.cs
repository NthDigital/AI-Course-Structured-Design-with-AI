using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Tests.Data;

public class RestaurantBookingDbContextTests : IDisposable
{
    private readonly RestaurantBookingDbContext _context;

    public RestaurantBookingDbContextTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantBookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RestaurantBookingDbContext(options);
    }

    [Fact]
    public async Task DbContext_Should_Create_Database_Schema()
    {
        // Act
        var canConnect = await _context.Database.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public void DbContext_Should_Have_All_Required_DbSets()
    {
        // Assert
        _context.RestaurantOwners.Should().NotBeNull();
        _context.Restaurants.Should().NotBeNull();
        _context.Tables.Should().NotBeNull();
        _context.Customers.Should().NotBeNull();
        _context.Reservations.Should().NotBeNull();
        _context.OperatingHours.Should().NotBeNull();
        _context.AvailabilityBlocks.Should().NotBeNull();
    }

    [Fact]
    public async Task DbContext_Should_Save_Restaurant_Entity()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        _context.RestaurantOwners.Add(owner);
        await _context.SaveChangesAsync();

        var restaurant = new Restaurant("Test Restaurant", "Italian", "Test Description", "123 Test St", "1234567890", owner.Id);

        // Act
        _context.Restaurants.Add(restaurant);
        var result = await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
        var savedRestaurant = await _context.Restaurants.FirstOrDefaultAsync();
        savedRestaurant.Should().NotBeNull();
        savedRestaurant!.Name.Should().Be("Test Restaurant");
    }

    [Fact]
    public async Task DbContext_Should_Enforce_Entity_Relationships()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        _context.RestaurantOwners.Add(owner);
        await _context.SaveChangesAsync();

        var restaurant = new Restaurant("Test Restaurant", "Italian", "Test Description", "123 Test St", "1234567890", owner.Id);
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        var table = new Table("T1", 4, restaurant.Id);

        // Act
        _context.Tables.Add(table);
        var result = await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
        var savedTable = await _context.Tables.FirstOrDefaultAsync();
        savedTable.Should().NotBeNull();
        savedTable!.RestaurantId.Should().Be(restaurant.Id);
        
        // Verify the relationship by querying the restaurant
        var relatedRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == savedTable.RestaurantId);
        relatedRestaurant.Should().NotBeNull();
        relatedRestaurant!.Name.Should().Be("Test Restaurant");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
