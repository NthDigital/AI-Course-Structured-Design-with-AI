using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Infrastructure.Data;

public class RestaurantBookingDbContext : DbContext
{
    public RestaurantBookingDbContext(DbContextOptions<RestaurantBookingDbContext> options) : base(options)
    {
    }

    public DbSet<RestaurantOwner> RestaurantOwners { get; set; } = null!;
    public DbSet<Restaurant> Restaurants { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<OperatingHours> OperatingHours { get; set; } = null!;
    public DbSet<AvailabilityBlock> AvailabilityBlocks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantBookingDbContext).Assembly);
    }
}
