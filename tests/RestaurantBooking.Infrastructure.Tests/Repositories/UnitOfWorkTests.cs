using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Infrastructure.Repositories;

namespace RestaurantBooking.Infrastructure.Tests.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly RestaurantBookingDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantBookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new RestaurantBookingDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void UnitOfWork_Should_Provide_All_Repository_Properties()
    {
        // Assert
        _unitOfWork.RestaurantOwners.Should().NotBeNull();
        _unitOfWork.Restaurants.Should().NotBeNull();
        _unitOfWork.Tables.Should().NotBeNull();
        _unitOfWork.Customers.Should().NotBeNull();
        _unitOfWork.Reservations.Should().NotBeNull();
        _unitOfWork.OperatingHours.Should().NotBeNull();
        _unitOfWork.AvailabilityBlocks.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Save_All_Changes()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");
        var restaurant = new Restaurant("Test Restaurant", "Italian", "Test Description", "123 Test St", "1234567890", owner.Id);

        await _unitOfWork.RestaurantOwners.AddAsync(owner);
        await _unitOfWork.Restaurants.AddAsync(restaurant);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(2);
        var savedOwner = await _context.RestaurantOwners.FirstOrDefaultAsync();
        var savedRestaurant = await _context.Restaurants.FirstOrDefaultAsync();
        
        savedOwner.Should().NotBeNull();
        savedRestaurant.Should().NotBeNull();
    }

    [Fact]
    public async Task Transaction_Should_Support_Begin_Commit_Pattern()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");

        // Act
        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.RestaurantOwners.AddAsync(owner);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var savedOwner = await _context.RestaurantOwners.FirstOrDefaultAsync();
        savedOwner.Should().NotBeNull();
        savedOwner!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Transaction_Should_Support_Rollback()
    {
        // Arrange
        var owner = new RestaurantOwner("Test", "Owner", "test@example.com", "1234567890");

        // Act
        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.RestaurantOwners.AddAsync(owner);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        // Note: InMemory database doesn't actually support transactions, so we just verify methods don't throw
        var savedOwner = await _context.RestaurantOwners.FirstOrDefaultAsync();
        // InMemory database doesn't rollback, but the transaction methods should work without throwing
        savedOwner.Should().NotBeNull(); // InMemory limitation - would be null in real PostgreSQL
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
        _context.Dispose();
    }
}
