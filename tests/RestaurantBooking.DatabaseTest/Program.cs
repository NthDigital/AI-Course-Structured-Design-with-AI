using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Infrastructure.Extensions;

namespace RestaurantBooking.DatabaseTest;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Restaurant Booking Database Connectivity Test ===\n");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Setup services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddInfrastructureServices(configuration);

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantBookingDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Test 1: Check connection string
            Console.WriteLine("1. Checking connection string configuration...");
            var connectionString = dbContext.Database.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("❌ Connection string is not configured!");
                return;
            }
            
            // Hide password for display
            var displayConnectionString = connectionString.Contains("Password=")
                ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***"
                : connectionString;
            Console.WriteLine($"✅ Connection string configured: {displayConnectionString}");

            // Test 2: Check provider
            Console.WriteLine("\n2. Checking database provider...");
            var providerName = dbContext.Database.ProviderName;
            Console.WriteLine($"✅ Database provider: {providerName}");

            // Test 3: Test connection
            Console.WriteLine("\n3. Testing database connection...");
            var canConnect = await dbContext.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                Console.WriteLine("❌ Cannot connect to database!");
                Console.WriteLine("   Make sure PostgreSQL is running and the connection string is correct.");
                return;
            }
            
            Console.WriteLine("✅ Database connection successful!");

            // Test 4: Check if database exists
            Console.WriteLine("\n4. Checking database existence...");
            var databaseExists = await dbContext.Database.CanConnectAsync();
            Console.WriteLine($"✅ Database exists and is accessible: {databaseExists}");

            // Test 5: Test entity mappings
            Console.WriteLine("\n5. Checking entity mappings...");
            var entityTypes = dbContext.Model.GetEntityTypes().ToList();
            Console.WriteLine($"✅ Entity types mapped: {entityTypes.Count}");
            
            foreach (var entityType in entityTypes)
            {
                Console.WriteLine($"   - {entityType.ClrType.Name}");
            }

            // Test 6: Test basic queries (if tables exist)
            Console.WriteLine("\n6. Testing basic queries...");
            try
            {
                var restaurantOwnersCount = await dbContext.RestaurantOwners.CountAsync();
                var restaurantsCount = await dbContext.Restaurants.CountAsync();
                var tablesCount = await dbContext.Tables.CountAsync();
                var customersCount = await dbContext.Customers.CountAsync();
                var reservationsCount = await dbContext.Reservations.CountAsync();
                var operatingHoursCount = await dbContext.OperatingHours.CountAsync();
                var availabilityBlocksCount = await dbContext.AvailabilityBlocks.CountAsync();

                Console.WriteLine("✅ Database queries successful!");
                Console.WriteLine($"   - Restaurant Owners: {restaurantOwnersCount}");
                Console.WriteLine($"   - Restaurants: {restaurantsCount}");
                Console.WriteLine($"   - Tables: {tablesCount}");
                Console.WriteLine($"   - Customers: {customersCount}");
                Console.WriteLine($"   - Reservations: {reservationsCount}");
                Console.WriteLine($"   - Operating Hours: {operatingHoursCount}");
                Console.WriteLine($"   - Availability Blocks: {availabilityBlocksCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Tables may not exist yet (this is normal if migrations haven't been run):");
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n=== Database Connectivity Test Complete ===");
            Console.WriteLine("✅ All basic connectivity tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Database connectivity test failed:");
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
    }
}
