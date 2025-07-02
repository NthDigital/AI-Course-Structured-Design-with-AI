namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRestaurantOwnerRepository RestaurantOwners { get; }
    IRestaurantRepository Restaurants { get; }
    ITableRepository Tables { get; }
    ICustomerRepository Customers { get; }
    IReservationRepository Reservations { get; }
    IOperatingHoursRepository OperatingHours { get; }
    IAvailabilityBlockRepository AvailabilityBlocks { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
