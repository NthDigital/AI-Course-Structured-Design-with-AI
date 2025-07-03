using Microsoft.EntityFrameworkCore.Storage;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly RestaurantBookingDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRestaurantOwnerRepository? _restaurantOwners;
    private IRestaurantRepository? _restaurants;
    private ITableRepository? _tables;
    private ICustomerRepository? _customers;
    private IReservationRepository? _reservations;
    private IOperatingHoursRepository? _operatingHours;
    private IAvailabilityBlockRepository? _availabilityBlocks;

    public UnitOfWork(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public IRestaurantOwnerRepository RestaurantOwners
    {
        get { return _restaurantOwners ??= new RestaurantOwnerRepository(_context); }
    }

    public IRestaurantRepository Restaurants
    {
        get { return _restaurants ??= new RestaurantRepository(_context); }
    }

    public ITableRepository Tables
    {
        get { return _tables ??= new TableRepository(_context); }
    }

    public ICustomerRepository Customers
    {
        get { return _customers ??= new CustomerRepository(_context); }
    }

    public IReservationRepository Reservations
    {
        get { return _reservations ??= new ReservationRepository(_context); }
    }

    public IOperatingHoursRepository OperatingHours
    {
        get { return _operatingHours ??= new OperatingHoursRepository(_context); }
    }

    public IAvailabilityBlockRepository AvailabilityBlocks
    {
        get { return _availabilityBlocks ??= new AvailabilityBlockRepository(_context); }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
