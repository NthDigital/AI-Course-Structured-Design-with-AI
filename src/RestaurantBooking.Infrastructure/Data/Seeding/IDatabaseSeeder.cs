using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Data.Seeding;

public interface IDatabaseSeeder
{
    Task SeedAsync();
}
