using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Data.Seeding;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly RestaurantBookingDbContext _context;

    public DatabaseSeeder(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if data already exists
        if (await _context.RestaurantOwners.AnyAsync())
        {
            return; // Database has been seeded
        }

        await SeedRestaurantOwnersAsync();
        await SeedCustomersAsync();
        await SeedRestaurantsAsync();
        await SeedTablesAsync();
        await SeedOperatingHoursAsync();

        await _context.SaveChangesAsync();
    }

    private async Task SeedRestaurantOwnersAsync()
    {
        var owners = new List<RestaurantOwner>
        {
            new("John", "Smith", "john.smith@example.com", "555-0101"),
            new("Maria", "Garcia", "maria.garcia@example.com", "555-0102"),
            new("David", "Johnson", "david.johnson@example.com", "555-0103")
        };

        await _context.RestaurantOwners.AddRangeAsync(owners);
        await _context.SaveChangesAsync(); // Save to get IDs
    }

    private async Task SeedCustomersAsync()
    {
        var customers = new List<Customer>
        {
            new("Alice", "Wilson", "alice.wilson@example.com", "555-0201"),
            new("Bob", "Davis", "bob.davis@example.com", "555-0202"),
            new("Carol", "Brown", "carol.brown@example.com", "555-0203"),
            new("Daniel", "Miller", "daniel.miller@example.com", "555-0204")
        };

        await _context.Customers.AddRangeAsync(customers);
    }

    private async Task SeedRestaurantsAsync()
    {
        var owners = await _context.RestaurantOwners.ToListAsync();
        
        var restaurants = new List<Restaurant>
        {
            new("Bella Italia", "Italian", "Authentic Italian cuisine with fresh ingredients", 
                "123 Main St, Downtown", "555-1001", owners[0].Id),
            new("Spice Garden", "Indian", "Traditional Indian dishes with aromatic spices", 
                "456 Oak Ave, Midtown", "555-1002", owners[1].Id),
            new("Ocean Breeze", "Seafood", "Fresh seafood restaurant with ocean views", 
                "789 Beach Rd, Coastal", "555-1003", owners[2].Id)
        };

        await _context.Restaurants.AddRangeAsync(restaurants);
        await _context.SaveChangesAsync(); // Save to get IDs
    }

    private async Task SeedTablesAsync()
    {
        var restaurants = await _context.Restaurants.ToListAsync();
        var tables = new List<Table>();

        foreach (var restaurant in restaurants)
        {
            // Add various table sizes for each restaurant
            tables.AddRange(new[]
            {
                new Table("T1", 2, restaurant.Id),
                new Table("T2", 2, restaurant.Id),
                new Table("T3", 4, restaurant.Id),
                new Table("T4", 4, restaurant.Id),
                new Table("T5", 6, restaurant.Id),
                new Table("T6", 8, restaurant.Id)
            });
        }

        await _context.Tables.AddRangeAsync(tables);
    }

    private async Task SeedOperatingHoursAsync()
    {
        var restaurants = await _context.Restaurants.ToListAsync();
        var operatingHours = new List<OperatingHours>();

        foreach (var restaurant in restaurants)
        {
            // Add standard operating hours (Monday-Sunday)
            for (int day = 0; day < 7; day++)
            {
                operatingHours.Add(new OperatingHours(
                    restaurant.Id,
                    (DayOfWeek)day,
                    new TimeOnly(11, 0), // 11:00 AM
                    new TimeOnly(22, 0)  // 10:00 PM
                ));
            }
        }

        await _context.OperatingHours.AddRangeAsync(operatingHours);
    }
}
