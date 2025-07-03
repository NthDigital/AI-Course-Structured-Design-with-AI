using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Infrastructure.Repositories;

namespace RestaurantBooking.Infrastructure.Tests.Repositories;

public class RestaurantRepositoryTests : IDisposable
{
    private readonly RestaurantBookingDbContext _context;
    private readonly IRestaurantRepository _repository;

    public RestaurantRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantBookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RestaurantBookingDbContext(options);
        _repository = new RestaurantRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnRestaurant()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        _context.RestaurantOwners.Add(owner);
        await _context.SaveChangesAsync();

        var restaurant = new Restaurant("Test Restaurant", "Italian", "Test Description", "123 Test St", "1234567890", owner.Id);
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(restaurant.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Restaurant");
        result.CuisineType.Should().Be("Italian");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidRestaurant_ShouldAddToDatabase()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        _context.RestaurantOwners.Add(owner);
        await _context.SaveChangesAsync();

        var restaurant = new Restaurant("New Restaurant", "Mexican", "New Description", "456 Test Ave", "9876543210", owner.Id);

        // Act
        var result = await _repository.AddAsync(restaurant);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Restaurant");

        var savedRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurant.Id);
        savedRestaurant.Should().NotBeNull();
        savedRestaurant!.Name.Should().Be("New Restaurant");
    }

    [Fact]
    public async Task GetByOwnerIdAsync_WithValidOwnerId_ShouldReturnOwnerRestaurants()
    {
        // Arrange
        var owner1 = new RestaurantOwner("Owner", "One", "owner1@example.com", "1234567890");
        var owner2 = new RestaurantOwner("Owner", "Two", "owner2@example.com", "0987654321");
        _context.RestaurantOwners.AddRange(owner1, owner2);
        await _context.SaveChangesAsync();

        var restaurant1 = new Restaurant("Restaurant 1", "Italian", "Description 1", "123 Test St", "1111111111", owner1.Id);
        var restaurant2 = new Restaurant("Restaurant 2", "Mexican", "Description 2", "456 Test Ave", "2222222222", owner1.Id);
        var restaurant3 = new Restaurant("Restaurant 3", "French", "Description 3", "789 Test Blvd", "3333333333", owner2.Id);

        _context.Restaurants.AddRange(restaurant1, restaurant2, restaurant3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByOwnerIdAsync(owner1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Restaurant 1");
        result.Should().Contain(r => r.Name == "Restaurant 2");
        result.Should().NotContain(r => r.Name == "Restaurant 3");
    }

    [Fact]
    public async Task UpdateAsync_WithValidChanges_ShouldUpdateRestaurant()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        _context.RestaurantOwners.Add(owner);
        await _context.SaveChangesAsync();

        var restaurant = new Restaurant("Original Name", "Italian", "Original Description", "123 Test St", "1234567890", owner.Id);
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        // Act
        restaurant.UpdateDetails("Updated Name", "Updated Description", "456 Updated St", "9876543210");
        await _repository.UpdateAsync(restaurant);
        await _context.SaveChangesAsync();

        // Assert
        var updatedRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurant.Id);
        updatedRestaurant.Should().NotBeNull();
        updatedRestaurant!.Name.Should().Be("Updated Name");
        updatedRestaurant.Description.Should().Be("Updated Description");
        updatedRestaurant.Address.Should().Be("456 Updated St");
        updatedRestaurant.PhoneNumber.Should().Be("9876543210");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRestaurants()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        _context.RestaurantOwners.Add(owner);
        await _context.SaveChangesAsync();

        var restaurant1 = new Restaurant("Restaurant 1", "Italian", "Description 1", "123 Test St", "1111111111", owner.Id);
        var restaurant2 = new Restaurant("Restaurant 2", "Mexican", "Description 2", "456 Test Ave", "2222222222", owner.Id);

        _context.Restaurants.AddRange(restaurant1, restaurant2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Restaurant 1");
        result.Should().Contain(r => r.Name == "Restaurant 2");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
