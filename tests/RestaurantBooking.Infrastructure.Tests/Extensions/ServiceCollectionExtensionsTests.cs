using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Infrastructure.Data.Seeding;
using RestaurantBooking.Infrastructure.Extensions;

namespace RestaurantBooking.Infrastructure.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_Should_Register_All_Required_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - DbContext
        serviceProvider.GetService<RestaurantBookingDbContext>().Should().NotBeNull();

        // Assert - Repositories
        serviceProvider.GetService<IRestaurantOwnerRepository>().Should().NotBeNull();
        serviceProvider.GetService<IRestaurantRepository>().Should().NotBeNull();
        serviceProvider.GetService<ITableRepository>().Should().NotBeNull();
        serviceProvider.GetService<ICustomerRepository>().Should().NotBeNull();
        serviceProvider.GetService<IReservationRepository>().Should().NotBeNull();
        serviceProvider.GetService<IOperatingHoursRepository>().Should().NotBeNull();
        serviceProvider.GetService<IAvailabilityBlockRepository>().Should().NotBeNull();

        // Assert - Unit of Work
        serviceProvider.GetService<IUnitOfWork>().Should().NotBeNull();

        // Assert - Database Seeder
        serviceProvider.GetService<IDatabaseSeeder>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructureServices_Should_Register_Services_With_Correct_Lifetimes()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert - All should be registered as Scoped
        var dbContextDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(RestaurantBookingDbContext));
        dbContextDescriptor.Should().NotBeNull();
        dbContextDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var unitOfWorkDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUnitOfWork));
        unitOfWorkDescriptor.Should().NotBeNull();
        unitOfWorkDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var repositoryDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IRestaurantRepository));
        repositoryDescriptor.Should().NotBeNull();
        repositoryDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }
}
