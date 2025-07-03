using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Infrastructure.Data.Seeding;
using RestaurantBooking.Infrastructure.Repositories;

namespace RestaurantBooking.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<RestaurantBookingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Add Repositories
        services.AddScoped<IRestaurantOwnerRepository, RestaurantOwnerRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IOperatingHoursRepository, OperatingHoursRepository>();
        services.AddScoped<IAvailabilityBlockRepository, AvailabilityBlockRepository>();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Database Seeding
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        return services;
    }
}
